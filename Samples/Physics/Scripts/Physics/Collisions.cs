using System.Collections.Generic;
using UnityEngine;

namespace Massive.Samples.Physics
{
	public static class Collisions
	{
		public const float PositionCollisionBeta = 0.7f;

		public static readonly List<Contact> Contacts = new List<Contact>();

		public static void Solve(MassiveDataSet<Rigidbody> bodies, MassiveDataSet<SphereCollider> spheres, MassiveDataSet<BoxCollider> boxes)
		{
			CollectContacts(bodies, spheres, boxes);

			foreach (Contact contact in Contacts)
			{
				ref var a = ref bodies.Get(contact.BodyA);
				ref var b = ref bodies.Get(contact.BodyB);

				SolveVelocityCollision(ref a, ref b, contact);
				SolvePositionCollision(ref a, ref b, contact);
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
				a.WorldCenterOfMass.Position -= resolution * a.InverseMass;

			if (!b.IsStatic)
				b.WorldCenterOfMass.Position += resolution * b.InverseMass;
		}

		private static void SolveVelocityCollision(ref Rigidbody a, ref Rigidbody b, Contact contact)
		{
			var e = contact.Restitution;
			var friction = contact.Friction;

			var normal = contact.Normal;

			Vector3 relativeVelocity = a.GetVelocityAtPoint(contact.ContactPointA) - b.GetVelocityAtPoint(contact.ContactPointB);

			Vector3 rA = contact.ContactPointA - a.WorldCenterOfMass.Position;
			Vector3 rB = contact.ContactPointB - b.WorldCenterOfMass.Position;

			Vector3 raCrossN = Vector3.Cross(rA, normal);
			Vector3 rbCrossN = Vector3.Cross(rB, normal);

			float angularInertiaA = a.IsStatic ? 0f : Vector3.Dot(raCrossN, a.InverseWorldInertiaTensor.MultiplyPoint3x4(raCrossN));
			float angularInertiaB = b.IsStatic ? 0f : Vector3.Dot(rbCrossN, b.InverseWorldInertiaTensor.MultiplyPoint3x4(rbCrossN));
			float angularInertia = angularInertiaA + angularInertiaB;

			float inverseMassA = a.IsStatic ? 0f : a.InverseMass;
			float inverseMassB = b.IsStatic ? 0f : b.InverseMass;
			float inverseMassSum = inverseMassA + inverseMassB;

			float inverseInertia = inverseMassSum + angularInertia;

			// Calculate normal impulse scalar
			float j = -(1 + e) * Vector3.Dot(relativeVelocity, normal) / inverseInertia;

			// Ignore non-separating impulse
			if (j < 0f)
			{
				return;
			}
			
			// Apply normal impulse
			Vector3 impulse = normal * j;
			a.ApplyImpulseAtPoint(impulse, contact.ContactPointA);
			b.ApplyImpulseAtPoint(-impulse, contact.ContactPointB);

			if (!a.IsStatic)
				Debug.DrawLine(contact.ContactPointA, contact.ContactPointA + impulse, Color.red, 0.5f);
			if (!b.IsStatic)
				Debug.DrawLine(contact.ContactPointB, contact.ContactPointB - impulse, Color.red, 0.5f);

			// Recalculate relative velocity after normal impulse for friction calculations
			relativeVelocity = a.GetVelocityAtPoint(contact.ContactPointA) - b.GetVelocityAtPoint(contact.ContactPointB);

			Vector3 tangent = default;
			Vector3 biTangent = default;
			Vector3.OrthoNormalize(ref normal, ref tangent, ref biTangent);

			float jt = -Vector3.Dot(relativeVelocity, tangent) / inverseInertia;
			float jbt = -Vector3.Dot(relativeVelocity, biTangent) / inverseInertia;

			// Correctly clamp the friction impulse magnitude based on the normal impulse
			float maxFrictionImpulse = j * friction;
			float tangentImpulseMagnitude = Mathf.Clamp(jt, -maxFrictionImpulse, maxFrictionImpulse);
			float biTangentImpulseMagnitude = Mathf.Clamp(jbt, -maxFrictionImpulse, maxFrictionImpulse);

			// Apply friction impulse
			Vector3 frictionImpulse = tangent * tangentImpulseMagnitude + biTangent * biTangentImpulseMagnitude;

			a.ApplyImpulseAtPoint(frictionImpulse, contact.ContactPointA);
			b.ApplyImpulseAtPoint(-frictionImpulse, contact.ContactPointB);
		}

		private static void CollectContacts(MassiveDataSet<Rigidbody> bodies, MassiveDataSet<SphereCollider> spheres, MassiveDataSet<BoxCollider> boxes)
		{
			Contacts.Clear();

			var aliveSpheres = spheres.AliveData;
			var aliveBoxes = boxes.AliveData;
			for (int i = 0; i < aliveSpheres.Length; ++i)
			{
				SphereCollider a = aliveSpheres[i];
				for (int j = i + 1; j < aliveSpheres.Length; ++j)
				{
					SphereCollider b = aliveSpheres[j];

					if (a.RigidbodyId == b.RigidbodyId || bodies.Get(a.RigidbodyId).IsStatic && bodies.Get(b.RigidbodyId).IsStatic)
					{
						continue;
					}

					var contact = new ColliderContact();
					CollisionTester.SphereVsSphere(ref a, ref b, b.World.Position - a.World.Position, ref contact);

					if (contact.Depth > 0f)
					{
						Vector3 contactPoint = a.World.Position + contact.OffsetFromColliderA;
						Contacts.Add(new Contact()
						{
							BodyA = a.RigidbodyId, BodyB = b.RigidbodyId,
							Normal = contact.Normal,
							Depth = contact.Depth,
							ContactPointA = contactPoint,
							ContactPointB = contactPoint,
							Friction = Mathf.Sqrt(a.Material.Friction * b.Material.Friction),
							Restitution = Mathf.Min(a.Material.Restitution, b.Material.Restitution)
						});
					}
				}

				for (int j = 0; j < aliveBoxes.Length; ++j)
				{
					ref BoxCollider b = ref aliveBoxes[j];

					if (a.RigidbodyId == b.RigidbodyId || bodies.Get(a.RigidbodyId).IsStatic && bodies.Get(b.RigidbodyId).IsStatic)
					{
						continue;
					}

					var contact = new ColliderContact();
					CollisionTester.SphereVsBox(ref a, ref b, b.World.Position - a.World.Position, b.World.Rotation, ref contact);

					if (contact.Depth > 0f)
					{
						Vector3 contactPoint = a.World.Position + contact.OffsetFromColliderA;
						Contacts.Add(new Contact()
						{
							BodyA = a.RigidbodyId, BodyB = b.RigidbodyId,
							Normal = contact.Normal,
							Depth = contact.Depth,
							ContactPointA = contactPoint,
							ContactPointB = contactPoint,
							Friction = Mathf.Sqrt(a.Material.Friction * b.Material.Friction),
							Restitution = Mathf.Min(a.Material.Restitution, b.Material.Restitution)
						});
					}
				}
			}

			for (int i = 0; i < aliveBoxes.Length; ++i)
			{
				BoxCollider a = aliveBoxes[i];
				for (int j = i + 1; j < aliveBoxes.Length; ++j)
				{
					BoxCollider b = aliveBoxes[j];

					if (a.RigidbodyId == b.RigidbodyId || bodies.Get(a.RigidbodyId).IsStatic && bodies.Get(b.RigidbodyId).IsStatic)
					{
						continue;
					}

					var gjkResult = GjkAlgorithm.Calculate(a, b);

					if (gjkResult.CollisionHappened)
					{
						var epaResult = EpaAlgorithm.Calculate(gjkResult.Simplex, a, b);

						Contacts.Add(new Contact()
						{
							BodyA = a.RigidbodyId, BodyB = b.RigidbodyId,
							Normal = -epaResult.PenetrationNormal,
							Depth = epaResult.PenetrationDepth,
							ContactPointA = epaResult.ContactFirst.Position,
							ContactPointB = epaResult.ContactSecond.Position,
							Friction = Mathf.Sqrt(a.Material.Friction * b.Material.Friction),
							Restitution = Mathf.Min(a.Material.Restitution, b.Material.Restitution)
						});
					}
				}
			}
		}
	}
}