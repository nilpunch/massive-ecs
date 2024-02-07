using System;
using System.Collections.Generic;
using UnityEngine;

namespace MassiveData.Samples.Physics
{
	public static class Collisions
	{
		public struct Contact
		{
			public int BodyA;
			public int BodyB;
			
			public Vector3 Normal;
			public float PenetrationDepth;
		}

		private static readonly List<Contact> s_contacts = new List<Contact>();
		
		public static void Solve(Massive<Rigidbody> rigidbodies, Massive<SphereCollider> colliders)
		{
			CollectContacts(colliders);

			foreach (Contact contact in s_contacts)
			{
				ref var a = ref rigidbodies.Get(contact.BodyA);
				ref var b = ref rigidbodies.Get(contact.BodyB);

				var e = Mathf.Min(a.Restitution, b.Restitution);

				var relativeVelocity = a.Velocity - b.Velocity;

				float inverseMassSum;

				if (!a.Static && !b.Static)
					inverseMassSum = a.InverseMass + b.InverseMass;
				else if (a.Static)
					inverseMassSum = b.InverseMass;
				else
					inverseMassSum = a.InverseMass;

				float impulseMagnitude = -(1 + e) * Vector3.Dot(relativeVelocity, contact.Normal) / inverseMassSum;
				Vector3 impulseDirection = contact.Normal;
				Vector3 impulse = impulseDirection * impulseMagnitude;
				
				a.ApplyImpulse(impulse);
				b.ApplyImpulse(-impulse);
			}
		}

		private static void CollectContacts(Massive<SphereCollider> colliders)
		{
			s_contacts.Clear();
			
			var aliveColliders = colliders.AliveData;
			for (int i = 0; i < aliveColliders.Length; ++i)
			{
				SphereCollider a = aliveColliders[i];

				for (int j = i + 1; j < aliveColliders.Length; ++j)
				{
					SphereCollider b = aliveColliders[j];
					
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