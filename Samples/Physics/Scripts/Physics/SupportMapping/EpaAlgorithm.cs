using System;
using System.Collections.Generic;
using UnityEngine;

namespace MassiveData.Samples.Physics
{
	public readonly struct Collision
	{
		public ContactPoint ContactFirst { get; }

		public ContactPoint ContactSecond { get; }

		public Vector3 PenetrationNormal { get; }

		public float PenetrationDepth { get; }

		public Collision(ContactPoint contactFirst, ContactPoint contactSecond, Vector3 penetrationNormal, float penetrationDepth)
		{
			ContactFirst = contactFirst;
			PenetrationNormal = penetrationNormal;
			PenetrationDepth = penetrationDepth;
			ContactSecond = contactSecond;
		}
	}
	
	public struct ContactPoint
	{
		public ContactPoint(Vector3 position)
		{
			Position = position;
		}

		public Vector3 Position { get; }
	}
	
	public static class EpaAlgorithm
	{
		private static float Tolerance => 0.0001f;

		public static List<Vector3> PolytopeShared { get; } = new List<Vector3>();
		public static List<Vector3> MinkowskiSharedA { get; } = new List<Vector3>();
		public static List<Vector3> MinkowskiSharedB { get; } = new List<Vector3>();
		public static List<PolytopeFace> PolytopeFacesShared { get; } = new List<PolytopeFace>();
		private static List<int> RemovalFacesIndicesShared { get; } = new List<int>();
		private static List<(int a, int b)> RemovalEdgesShared { get; } = new List<(int a, int b)>();

		public static Vector3 Barycentric(Vector3 a, Vector3 b, Vector3 c, Vector3 point, bool clamp = false)
		{
			Vector3 v0 = b - a;
			Vector3 v1 = c - a;
			Vector3 v2 = point - a;
			float d00 = Vector3.Dot(v0, v0);
			float d01 = Vector3.Dot(v0, v1);
			float d11 = Vector3.Dot(v1, v1);
			float d20 = Vector3.Dot(v2, v0);
			float d21 = Vector3.Dot(v2, v1);
			float denominator = d00 * d11 - d01 * d01;
			float v = (d11 * d20 - d01 * d21) / denominator;
			float w = (d00 * d21 - d01 * d20) / denominator;
			float u = 1f - v - w;

			return new Vector3(u, v, w);
		}

		private static Vector3 ProjectedBarycentric( Vector3 p, Vector3 q, Vector3 u, Vector3 v)
		{
			Vector3 n= Vector3.Cross( u, v );
			float oneOver4ASquared= 1f / Vector3.SqrMagnitude(n);
			Vector3 w= p - q;
			float c = Vector3.Dot( Vector3.Cross( u, w ), n ) * oneOver4ASquared;
			float b = Vector3.Dot( Vector3.Cross( w, v ), n ) * oneOver4ASquared;
			float a = 1f - b - c;

			return new Vector3(a, b, c);
		}

		public struct PolytopeFace
		{
			public PolytopeFace(int a, int b, int c)
			{
				A = a;
				B = b;
				C = c;
			}

			public int A { get; }
			public int B { get; }
			public int C { get; }
		}

		public static bool ApproximatelyEqual(float a, float b, float epsilon)
		{
			var difference = Mathf.Abs(a - b);
			return difference <= epsilon || difference <= Mathf.Max(Mathf.Abs(a), Mathf.Abs(b)) * epsilon;
		}

		public static Collision Calculate<T>(Simplex simplex, T shapeA,
			T shapeB, int maxIterations = 100) where T : ISupportMappable
		{
			PolytopeShared.Clear();
			PolytopeFacesShared.Clear();
			MinkowskiSharedA.Clear();
			MinkowskiSharedB.Clear();

			PolytopeShared.Add(simplex.A.Difference);
			MinkowskiSharedA.Add(simplex.A.SupportA);
			MinkowskiSharedB.Add(simplex.A.SupportB);
			PolytopeShared.Add(simplex.B.Difference);
			MinkowskiSharedA.Add(simplex.B.SupportA);
			MinkowskiSharedB.Add(simplex.B.SupportB);
			PolytopeShared.Add(simplex.C.Difference);
			MinkowskiSharedA.Add(simplex.C.SupportA);
			MinkowskiSharedB.Add(simplex.C.SupportB);
			PolytopeShared.Add(simplex.D.Difference);
			MinkowskiSharedA.Add(simplex.D.SupportA);
			MinkowskiSharedB.Add(simplex.D.SupportB);

			PolytopeFacesShared.Add(new PolytopeFace(0, 1, 2));
			PolytopeFacesShared.Add(new PolytopeFace(0, 1, 3));
			PolytopeFacesShared.Add(new PolytopeFace(0, 2, 3));
			PolytopeFacesShared.Add(new PolytopeFace(1, 2, 3));
			
			FixNormals(PolytopeShared, PolytopeFacesShared);

			(int index, float distance, Vector3 normal, PolytopeFace face) closestFace = default;
			
			int iteration = 1;
			while (iteration < maxIterations)
			{
				closestFace = FindClosestFace(PolytopeShared, PolytopeFacesShared);

				MinkowskiDifference supportPoint = MinkowskiDifference.Calculate(shapeA, shapeB, closestFace.normal);

				float minkowskiDistance = Vector3.Dot(closestFace.normal, supportPoint.Difference);
				float closestFaceDistance = closestFace.distance;

				if (ApproximatelyEqual(closestFaceDistance, minkowskiDistance, Tolerance))
				{
					break;
				}

				PolytopeShared.Add(supportPoint.Difference);
				MinkowskiSharedA.Add(supportPoint.SupportA);
				MinkowskiSharedB.Add(supportPoint.SupportB);
				ExpandPolytope(PolytopeShared, PolytopeFacesShared, supportPoint.Difference);
				
				iteration += 1;
			}
			
			if (iteration >= maxIterations)
			{
				throw new Exception();
			}

			Vector3 barycentric = Barycentric(
				PolytopeShared[closestFace.face.A],
				PolytopeShared[closestFace.face.B],
				PolytopeShared[closestFace.face.C],
				closestFace.normal * closestFace.distance);

			Vector3 supportAA = MinkowskiSharedA[closestFace.face.A];
			Vector3 supportAB = MinkowskiSharedA[closestFace.face.B];
			Vector3 supportAC = MinkowskiSharedA[closestFace.face.C];
			Vector3 supportBA = MinkowskiSharedB[closestFace.face.A];
			Vector3 supportBB = MinkowskiSharedB[closestFace.face.B];
			Vector3 supportBC = MinkowskiSharedB[closestFace.face.C];

			Vector3 point1 = barycentric.x * supportAA + barycentric.y * supportAB + barycentric.z * supportAC;
			Vector3 point2 = barycentric.x * supportBA + barycentric.y * supportBB + barycentric.z * supportBC;

			return new Collision(new ContactPoint(point1), new ContactPoint(point2), closestFace.normal, closestFace.distance + Tolerance);
		}

		public static void ExpandPolytope(List<Vector3> polytope, List<PolytopeFace> faces, Vector3 extendPoint)
		{
			RemovalFacesIndicesShared.Clear();

			for (int i = 0; i < faces.Count; i++)
			{
				var face = faces[i];

				var ab = polytope[face.B] - polytope[face.A];
				var ac = polytope[face.C] - polytope[face.A];
				var normal = Vector3.Normalize(Vector3.Cross(ab, ac));

				if (Vector3.Dot(polytope[face.A], normal) < -Tolerance)
				{
					normal = -normal;
				}

				if (Vector3.Dot(normal, extendPoint - polytope[face.A]) > Tolerance)
				{
					RemovalFacesIndicesShared.Add(i);
				}
			}

			// Get the edges that are not shared between the faces that should be removed
			RemovalEdgesShared.Clear();
			foreach (int removalFaceIndex in RemovalFacesIndicesShared)
			{
				var face = faces[removalFaceIndex];
				(int a, int b) edgeAB = (face.A, face.B);
				(int a, int b) edgeAC = (face.A, face.C);
				(int a, int b) edgeBC = (face.B, face.C);

				AddOrDeleteEdge(RemovalEdgesShared, edgeAB);
				AddOrDeleteEdge(RemovalEdgesShared, edgeAC);
				AddOrDeleteEdge(RemovalEdgesShared, edgeBC);
			}

			// Remove the faces from the polytope
			for (int i = RemovalFacesIndicesShared.Count - 1; i >= 0; i--)
			{
				int index = RemovalFacesIndicesShared[i];
				faces.RemoveAt(index);
			}

			// Form new faces with the edges and new point
			Vector3 center = PolytopeCenter(polytope);
			foreach ((int a, int b) in RemovalEdgesShared)
			{
				var fixedFace = FixFaceNormal(new PolytopeFace(a, b, polytope.Count - 1), polytope, center);

				faces.Add(fixedFace);
			}
		}

		public static (int index, float distance, Vector3 normal, PolytopeFace face) FindClosestFace(
			List<Vector3> polytope, List<PolytopeFace> faces)
		{
			(int index, float distance, Vector3 normal, PolytopeFace face) closest = (-1, float.MaxValue,
				default, default);

			for (int i = 0; i < faces.Count; i++)
			{
				var face = faces[i];

				var ab = polytope[face.B] - polytope[face.A];
				var ac = polytope[face.C] - polytope[face.A];

				var normal = Vector3.Normalize(Vector3.Cross(ab, ac));
				var distance = Vector3.Dot(polytope[face.A], normal);

				if (distance < -Tolerance)
				{
					normal = -normal;
					distance = -distance;
				}

				if (distance < closest.distance)
				{
					closest = (i, distance, normal, face);
				}
			}

			return closest;
		}

		public static void FixNormals(List<Vector3> polytope, List<PolytopeFace> faces)
		{
			Vector3 center = PolytopeCenter(polytope);

			for (int i = 0; i < faces.Count; i++)
			{
				faces[i] = FixFaceNormal(faces[i], polytope, center);
			}
		}

		private static PolytopeFace FixFaceNormal(PolytopeFace face, List<Vector3> polytope, Vector3 center)
		{
			var ab = polytope[face.B] - polytope[face.A];
			var ac = polytope[face.C] - polytope[face.A];

			var normal = Vector3.Normalize(Vector3.Cross(ab, ac));

			if (Vector3.Dot(-center, normal) < 0f)
			{
				return new PolytopeFace(face.A, face.C, face.B);
			}

			return face;
		}

		private static Vector3 PolytopeCenter(List<Vector3> polytope)
		{
			Vector3 center = Vector3.zero;
			foreach (var vertex in polytope)
				center += vertex;
			center /= polytope.Count;
			return center;
		}

		public static void AddOrDeleteEdge(List<(int a, int b)> edges, (int a, int b) edge)
		{
			int edgeIndex = -1;

			for (int index = 0; index < edges.Count; index++)
			{
				(int a, int b) pair = edges[index];

				if (pair.a == edge.a && pair.b == edge.b || pair.a == edge.b && pair.b == edge.a)
				{
					edgeIndex = index;
					break;
				}
			}

			if (edgeIndex != -1)
			{
				edges.RemoveAt(edgeIndex);
			}
			else
			{
				edges.Add(edge);
			}
		}
	}
}