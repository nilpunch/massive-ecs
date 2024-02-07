using System;
using System.Collections.Generic;
using UnityEngine;

namespace MassiveData.Samples.Physics
{
	public static class Collisions
	{
		public const float PositionCollisionBeta = 0.7f;
		
		public struct Contact
		{
			public int BodyA;
			public int BodyB;
			
			public Vector3 Normal;
			public float PenetrationDepth;
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

			Vector3 resolution = -0.5f * PositionCollisionBeta * systemMass * contact.PenetrationDepth * contact.Normal;
			
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
		}
		
		private static readonly List<Contact> s_contacts = new List<Contact>();

		private static void CollectContacts(Massive<SphereCollider> colliders, Massive<Rigidbody> bodies)
		{
			s_contacts.Clear();
			
			var aliveColliders = colliders.AliveData;
			for (int i = 0; i < aliveColliders.Length; ++i)
			{
				SphereCollider a = aliveColliders[i];

				for (int j = i + 1; j < aliveColliders.Length; ++j)
				{
					SphereCollider b = aliveColliders[j];

					if (bodies.Get(a.RigidbodyId).IsStatic && bodies.Get(b.RigidbodyId).IsStatic)
					{
						continue;
					}
					
					Vector3 displacement = a.WorldPosition - b.WorldPosition;
					float sqrDistance = displacement.sqrMagnitude;
					float minDistance = a.Radius + b.Radius;

					if (sqrDistance < minDistance * minDistance)
					{
						float distance = Mathf.Sqrt(sqrDistance);
						Vector3 normal = displacement / distance;
						
						s_contacts.Add(new Contact()
						{
							BodyA = a.RigidbodyId,
							BodyB = b.RigidbodyId,
							Normal = normal,
							PenetrationDepth = (distance - minDistance) * 0.5f,
						});
					}
				}
			}
		}
	}
}