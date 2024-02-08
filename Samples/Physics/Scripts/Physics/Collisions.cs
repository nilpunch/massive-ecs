using System;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

namespace MassiveData.Samples.Physics
{
	public struct Contact
	{
		public int BodyA;
		public int BodyB;

		public Vector3 OffsetFromA;
		public Vector3 Normal;
		public float Depth;
	}

	public static class Collisions
	{
		public const float PositionCollisionBeta = 0.7f;

		public static readonly List<Contact> Contacts = new List<Contact>();

		public static void Solve(Massive<Rigidbody> bodies, Massive<SphereCollider> spheres, Massive<BoxCollider> boxes)
		{
			CollectContacts(bodies, spheres, boxes);

			foreach (Contact contact in Contacts)
			{
				ref var a = ref bodies.Get(contact.BodyA);
				ref var b = ref bodies.Get(contact.BodyB);

				SolvePositionCollision(ref a, ref b, contact);
				SolveVelocityCollision(ref a, ref b, contact);
			}
		}

		private static void CollectContacts(Massive<Rigidbody> bodies, Massive<SphereCollider> spheres, Massive<BoxCollider> boxes)
		{
			Contacts.Clear();

			var aliveSpheres = spheres.AliveData;
			var aliveBoxes = boxes.AliveData;
			for (int i = 0; i < aliveSpheres.Length; ++i)
			{
				ref SphereCollider a = ref aliveSpheres[i];
				for (int j = i + 1; j < aliveSpheres.Length; ++j)
				{
					ref SphereCollider b = ref aliveSpheres[j];

					if (bodies.Get(a.RigidbodyId).IsStatic && bodies.Get(b.RigidbodyId).IsStatic)
					{
						continue;
					}

					var contact = new Contact() { BodyA = a.RigidbodyId, BodyB = b.RigidbodyId };
					CollisionTester.SphereVsSphere(ref a, ref b, b.WorldPosition - a.WorldPosition, ref contact);

					if (contact.Depth > 0f)
					{
						Contacts.Add(contact);
					}
				}
				
				for (int j = 0; j < aliveBoxes.Length; ++j)
				{
					ref BoxCollider b = ref aliveBoxes[j];

					if (bodies.Get(a.RigidbodyId).IsStatic && bodies.Get(b.RigidbodyId).IsStatic)
					{
						continue;
					}

					var contact = new Contact() { BodyA = a.RigidbodyId, BodyB = b.RigidbodyId };
					CollisionTester.SphereVsBox(ref a, ref b, b.WorldPosition - a.WorldPosition, b.WorldRotation, ref contact);

					if (contact.Depth > 0f)
					{
						Contacts.Add(contact);
					}
				}
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
	}

	//Individual pair testers are designed to be used outside of the narrow phase. They need to be usable for queries and such, so all necessary data must be gathered externally.
}