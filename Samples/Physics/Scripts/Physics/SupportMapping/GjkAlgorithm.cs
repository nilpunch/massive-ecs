using System;
using UnityEngine;

namespace Massive.Samples.Physics
{
	public struct Simplex
	{
		public MinkowskiDifference A;
		public MinkowskiDifference B;
		public MinkowskiDifference C;
		public MinkowskiDifference D;

		public int Stage;
	}

	public static class GjkAlgorithm
	{
		private static float Tolerance => 0.0001f;

		public struct Result
		{
			public Result(bool collisionHappened, Simplex simplex, int iterations, Vector3 direction)
			{
				CollisionHappened = collisionHappened;
				Simplex = simplex;
				Iterations = iterations;
				Direction = direction;
			}

			public bool CollisionHappened { get; }
			public Simplex Simplex { get; }

			public int Iterations { get; }
			public Vector3 Direction { get; }
		}

		public static Result Calculate<T>(T shapeA, T shapeB, int maxIterations = 100) where T : ISupportMappable
		{
			var simplex = new Simplex();

			Vector3 direction = NormalizeSafe(shapeB.Centre - shapeA.Centre, Vector3.up);

			bool colliding = false;
			int iterations = 1;
			while (iterations < maxIterations)
			{
				MinkowskiDifference supportPoint = MinkowskiDifference.Calculate(shapeA, shapeB, direction);

				simplex.D = simplex.C;
				simplex.C = simplex.B;
				simplex.B = simplex.A;
				simplex.A = supportPoint;

				if (Vector3.Dot(supportPoint.Difference, direction) <= 0f)
				{
					break;
				}

				var encloseResult = TryEncloseOrigin(ref simplex, shapeA, shapeB, direction);

				if (encloseResult.EncloseOrigin)
				{
					colliding = true;
					break;
				}

				direction = encloseResult.NextDirection;
				simplex.Stage += 1;
				iterations += 1;
			}

			if (iterations >= maxIterations)
			{
				throw new Exception();
			}

			return new Result(colliding, simplex, iterations, direction);
		}

		private static Vector3 NormalizeSafe(Vector3 vector, Vector3 defaultValue)
		{
			Vector3 result = Vector3.Normalize(vector);

			if (result.Equals(Vector3.zero))
			{
				return defaultValue;
			}

			return result;
		}

		public static unsafe float CopySign(float target, float source)
		{
			// Convert floats to their bitwise integer representations
			uint targetBits = *(uint*)&target;
			uint sourceBits = *(uint*)&source;

			// Clear the sign bit of the target and then set it to the sign bit of the source
			uint resultBits = (targetBits & 0x7FFFFFFF) | (sourceBits & 0x80000000);

			// Convert the bits back to a float
			return *(float*)&resultBits;
		}

		private static Vector3 Orthonormal(Vector3 a)
		{
			float length = a.magnitude;
			float s = CopySign(length, a.z);
			float h = a.z + s;
			return new Vector3(s * h - a.x * a.x, -a.x * a.y, -a.x * h);
		}

		private static Vector3 TripleProduct(Vector3 a, Vector3 b, Vector3 c)
		{
			return Vector3.Cross(Vector3.Cross(a, b), c);
		}

		private static (bool EncloseOrigin, Vector3 NextDirection) TryEncloseOrigin<T>(ref Simplex simplex,
			T shapeA, T shapeB, Vector3 direction) where T : ISupportMappable
		{
			switch (simplex.Stage)
			{
				case 0:
				{
					direction = NormalizeSafe(shapeB.Centre - shapeA.Centre, Vector3.up);
					break;
				}
				case 1:
				{
					// Flip the direction
					direction = -direction;
					break;
				}
				case 2:
				{
					// Line ab is the line formed by the first two vertices
					Vector3 ab = simplex.B.Difference - simplex.A.Difference;
					// Line a0 is the line from the first vertex to the origin
					Vector3 a0 = -simplex.A.Difference;

					if (Vector3.Cross(ab, a0) == Vector3.zero)
						direction = Orthonormal(ab);
					else
						// Use the triple-cross-product to calculate a direction perpendicular
						// To line ab in the direction of the origin
						direction = TripleProduct(ab, a0, ab);
					break;
				}
				case 3:
				{
					Vector3 ab = simplex.B.Difference - simplex.A.Difference;
					Vector3 ac = simplex.C.Difference - simplex.A.Difference;
					direction = Vector3.Cross(ab, ac);

					// Ensure it points toward the origin
					Vector3 a0 = -simplex.A.Difference;
					if (Vector3.Dot(direction, a0) < 0f)
						direction = -direction;
					break;
				}
				case 4:
				{
					// Calculate edges of interest
					Vector3 ab = simplex.B.Difference - simplex.A.Difference;
					Vector3 ac = simplex.C.Difference - simplex.A.Difference;
					Vector3 ad = simplex.D.Difference - simplex.A.Difference;

					Vector3 bc = simplex.C.Difference - simplex.B.Difference;
					Vector3 bd = simplex.D.Difference - simplex.B.Difference;
					Vector3 ba = -ab;

					// ABC
					direction = Vector3.Normalize(Vector3.Cross(ab, ac));
					if (Vector3.Dot(ad, direction) > 0f)
					{
						direction = -direction;
					}

					if (Vector3.Dot(simplex.A.Difference, direction) < -Tolerance)
					{
						// Remove d
						simplex.Stage = 3;
						return (false, direction);
					}

					// ADB
					direction = Vector3.Normalize(Vector3.Cross(ab, ad));
					if (Vector3.Dot(ac, direction) > 0f)
					{
						direction = -direction;
					}

					if (Vector3.Dot(simplex.A.Difference, direction) < -Tolerance)
					{
						// Remove c
						simplex.C = simplex.D;
						simplex.Stage = 3;
						return (false, direction);
					}

					// ACD
					direction = Vector3.Normalize(Vector3.Cross(ac, ad));
					if (Vector3.Dot(ab, direction) > 0f)
					{
						direction = -direction;
					}

					if (Vector3.Dot(simplex.A.Difference, direction) < -Tolerance)
					{
						// Remove b
						simplex.B = simplex.C;
						simplex.C = simplex.D;
						simplex.Stage = 3;
						return (false, direction);
					}

					// BCD
					direction = Vector3.Normalize(Vector3.Cross(bc, bd));
					if (Vector3.Dot(ba, direction) > 0f)
					{
						direction = -direction;
					}

					if (Vector3.Dot(simplex.B.Difference, direction) < -Tolerance)
					{
						// Remove a
						simplex.A = simplex.B;
						simplex.B = simplex.C;
						simplex.C = simplex.D;
						simplex.Stage = 3;
						return (false, direction);
					}

					// origin is in center
					return (true, direction);
				}
			}

			return (false, direction);
		}
	}
}