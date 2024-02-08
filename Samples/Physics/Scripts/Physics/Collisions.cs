using System;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.CompilerServices;
using UnityEngine;
using Matrix4x4 = UnityEngine.Matrix4x4;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;

namespace MassiveData.Samples.Physics
{
	public static class Collisions
	{
		public const float PositionCollisionBeta = 0.7f;

		public struct Contact
		{
			public int BodyA;
			public int BodyB;

			public Vector3 OffsetFromA;
			public Vector3 Normal;
			public float Depth;
		}

		public static void Solve(Massive<SphereCollider> colliders, Massive<Rigidbody> bodies)
		{
			CollectContacts(colliders, bodies);

			foreach (Contact contact in s_contacts)
			{
				ref var a = ref bodies.Get(contact.BodyA);
				ref var b = ref bodies.Get(contact.BodyB);

				SolvePositionCollision(ref a, ref b, contact);
				SolveVelocityCollision(ref a, ref b, contact);
			}
		}

		private static void SolvePositionCollision(ref Rigidbody a, ref Rigidbody b, Contact contact)
		{
			float systemMass;
			if (!a.IsStatic && !b.IsStatic)
				systemMass = a.Mass + b.Mass;
			else if (a.IsStatic)
				systemMass = b.Mass + b.Mass;
			else
				systemMass = a.Mass + a.Mass;

			Vector3 resolution = -0.5f * PositionCollisionBeta * systemMass * contact.Depth * contact.Normal;

			if (!a.IsStatic)
				a.Position += resolution * a.InverseMass;

			if (!b.IsStatic)
				b.Position -= resolution * b.InverseMass;
		}

		private static void SolveVelocityCollision(ref Rigidbody a, ref Rigidbody b, Contact contact)
		{
			var e = Mathf.Min(a.Restitution, b.Restitution);

			var relativeVelocity = a.Velocity - b.Velocity;

			float inverseMassSum;
			if (!a.IsStatic && !b.IsStatic)
				inverseMassSum = a.InverseMass + b.InverseMass;
			else if (a.IsStatic)
				inverseMassSum = b.InverseMass;
			else
				inverseMassSum = a.InverseMass;

			float impulseMagnitude = -(1 + e) * Vector3.Dot(relativeVelocity, contact.Normal) / inverseMassSum;
			Vector3 impulseDirection = contact.Normal;
			Vector3 impulse = impulseDirection * impulseMagnitude;

			a.ApplyImpulse(impulse);
			b.ApplyImpulse(-impulse);

			// // Calculate angular impulse
			// Vector3 rA = contact.OffsetFromA - a.Position;
			// Vector3 rB = contact.OffsetFromA - b.Position;
			//
			// // Calculate relative position from center of mass
			// Vector3 rAcm = rA - a.CenterOfMass;
			// Vector3 rBcm = rB - b.CenterOfMass;
			//
			// // Calculate torque
			// Vector3 torqueA = Vector3.Cross(rAcm, impulse);
			// Vector3 torqueB = Vector3.Cross(rBcm, -impulse);
			//
			// // Calculate change in angular velocity (angular impulse)
			// Vector3 angularImpulseA = Vector3.Scale(torqueA, a.InverseInertiaTensor);
			// Vector3 angularImpulseB = Vector3.Scale(torqueB, b.InverseInertiaTensor);
			//
			// // Apply angular impulse
			// a.ApplyAngularImpulse(angularImpulseA);
			// b.ApplyAngularImpulse(angularImpulseB);
		}

		private static readonly List<Contact> s_contacts = new List<Contact>();

		private static void CollectContacts(Massive<SphereCollider> colliders, Massive<Rigidbody> bodies)
		{
			s_contacts.Clear();

			var aliveColliders = colliders.AliveData;
			for (int i = 0; i < aliveColliders.Length; ++i)
			{
				ref SphereCollider a = ref aliveColliders[i];
				for (int j = i + 1; j < aliveColliders.Length; ++j)
				{
					ref SphereCollider b = ref aliveColliders[j];

					if (bodies.Get(a.RigidbodyId).IsStatic && bodies.Get(b.RigidbodyId).IsStatic)
					{
						continue;
					}

					var contact = new Contact() { BodyA = a.RigidbodyId, BodyB = b.RigidbodyId };
					CollisionTester.SphereVsSphere(ref a, ref b, b.WorldPosition - a.WorldPosition, ref contact);

					if (contact.Depth > 0f)
					{
						s_contacts.Add(contact);
					}
				}
			}
		}
	}

	//Individual pair testers are designed to be used outside of the narrow phase. They need to be usable for queries and such, so all necessary data must be gathered externally.
	public static class CollisionTester
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void SphereVsSphere(ref SphereCollider a, ref SphereCollider b, Vector3 offsetToB, ref Collisions.Contact contact)
		{
			var centerDistance = offsetToB.magnitude;
			//Note the negative 1. By convention, the normal points from B to A

			// Calculate the normal
			contact.Normal = offsetToB / centerDistance;

			// Determine if the normal is valid
			bool normalIsValid = centerDistance > 0f;

			// Arbitrarily choose the (0,1,0) if the two spheres are in the same position
			// Any unit length vector is equally valid
			if (!normalIsValid)
				contact.Normal = Vector3.up;

			// Calculate depth
			contact.Depth = a.Radius + b.Radius - centerDistance;

			// Calculate the contact position relative to object A
			float negativeOffsetFromA = contact.Depth * 0.5f - a.Radius;

			contact.OffsetFromA = contact.Normal * negativeOffsetFromA;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void SphereVsBox(ref SphereCollider a, ref BoxCollider b, Vector3 offsetToB, Quaternion orientationOfB, ref Collisions.Contact contact)
		{
			// Clamp the position of the sphere to the box.
			var orientationMatrixB = Matrix4x4.Rotate(orientationOfB);
			var orientationMatrixBTranspose = orientationMatrixB.transpose;

			// Note that we're working with localOffsetB, which is the offset from A to B, even though conceptually we want to be operating on the offset from B to A
			// Those offsets differ only by their sign, so are equivalent due to the symmetry of the box. The negation is left implicit
			var localOffsetB = orientationMatrixBTranspose.MultiplyVector(offsetToB);
			Vector3 clampedLocalOffsetB;
			clampedLocalOffsetB.x = Mathf.Min(Mathf.Max(localOffsetB.x, -b.HalfSize.x), b.HalfSize.x);
			clampedLocalOffsetB.y = Mathf.Min(Mathf.Max(localOffsetB.y, -b.HalfSize.y), b.HalfSize.y);
			clampedLocalOffsetB.z = Mathf.Min(Mathf.Max(localOffsetB.z, -b.HalfSize.z), b.HalfSize.z);

			// Implicit negation to make the normal point from B to A, following convention
			var outsideNormal = clampedLocalOffsetB - localOffsetB;
			var distance = outsideNormal.magnitude;
			var inverseDistance = 1f / distance;
			outsideNormal *= inverseDistance;
			var outsideDepth = a.Radius - distance;

			// If the sphere center is inside the box, then the shortest local axis to exit must be chosen
			var depthX = b.HalfSize.x - Mathf.Abs(localOffsetB.x);
			var depthY = b.HalfSize.y - Mathf.Abs(localOffsetB.y);
			var depthZ = b.HalfSize.z - Mathf.Abs(localOffsetB.z);
			var insideDepth = Mathf.Min(depthX, Mathf.Min(depthY, depthZ));

			// Only one axis may have a nonzero component.
			bool useX = insideDepth == depthX;
			bool useY = insideDepth == depthY && !useX;
			bool useZ = !(useX || useY);

			var insideNormal = Vector3.zero;

			if (useX)
				insideNormal.x = (localOffsetB.x < 0f) ? 1f : -1f;
			if (useY)
				insideNormal.y = (localOffsetB.y < 0f) ? 1f : -1f;
			if (useZ)
				insideNormal.z = (localOffsetB.z < 0f) ? 1f : -1f;

			insideDepth += a.Radius;
			var useInside = distance == 0f;
			var localNormal = useInside ? insideNormal : outsideNormal;
			contact.Normal = orientationMatrixB.MultiplyVector(localNormal);

			contact.Depth = useInside ? insideDepth : outsideDepth;

			//The contact position relative to object A (the sphere) is computed as the average of the extreme point along the normal toward the opposing shape on each shape, averaged.
			//For capsule-sphere, this can be computed from the normal and depth.
			var negativeOffsetFromSphere = contact.Depth * 0.5f - a.Radius;
			contact.OffsetFromA = contact.Normal * negativeOffsetFromSphere;
		}
	}
}