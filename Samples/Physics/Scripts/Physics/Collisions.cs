using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

namespace MassiveData.Samples.Physics
{
	public static class Collisions
	{
		public const float PositionCollisionBeta = 0.7f;

		public static readonly List<RigidbodyContact> Contacts = new List<RigidbodyContact>();

		public static void Solve(Massive<Rigidbody> bodies, Massive<SphereCollider> spheres, Massive<BoxCollider> boxes)
		{
			CollectContacts(bodies, spheres, boxes);

			foreach (RigidbodyContact contact in Contacts)
			{
				ref var a = ref bodies.Get(contact.BodyA);
				ref var b = ref bodies.Get(contact.BodyB);

				SolveVelocityCollision(ref a, ref b, contact);
				SolvePositionCollision(ref a, ref b, contact);
			}
		}

		private static void CollectContacts(Massive<Rigidbody> bodies, Massive<SphereCollider> spheres, Massive<BoxCollider> boxes)
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
					CollisionTester.SphereVsSphere(ref a, ref b, b.WorldPosition - a.WorldPosition, ref contact);

					if (contact.Depth > 0f)
					{
						Vector3 contactPoint = a.TransformFromLocalToWorld(contact.OffsetFromColliderA);
						Contacts.Add(new RigidbodyContact()
						{
							BodyA = a.RigidbodyId, BodyB = b.RigidbodyId,
							Normal = contact.Normal,
							Depth = contact.Depth,
							ContactPointA = contactPoint,
							ContactPointB = contactPoint
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
					CollisionTester.SphereVsBox(ref a, ref b, b.WorldPosition - a.WorldPosition, b.WorldRotation, ref contact);

					if (contact.Depth > 0f)
					{
						Vector3 contactPoint = a.TransformFromLocalToWorld(contact.OffsetFromColliderA);
						Contacts.Add(new RigidbodyContact()
						{
							BodyA = a.RigidbodyId, BodyB = b.RigidbodyId,
							Normal = contact.Normal,
							Depth = contact.Depth,
							ContactPointA = contactPoint,
							ContactPointB = contactPoint
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

						Contacts.Add(new RigidbodyContact()
						{
							BodyA = a.RigidbodyId, BodyB = b.RigidbodyId,
							Normal = -epaResult.PenetrationNormal,
							Depth = epaResult.PenetrationDepth,
							ContactPointA = epaResult.ContactFirst.Position,
							ContactPointB = epaResult.ContactSecond.Position
						});
					}
				}
			}
		}

		private static void SolvePositionCollision(ref Rigidbody a, ref Rigidbody b, RigidbodyContact contact)
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
				a.Position -= resolution * a.InverseMass;

			if (!b.IsStatic)
				b.Position += resolution * b.InverseMass;
		}

		private static void SolveVelocityCollision(ref Rigidbody a, ref Rigidbody b, RigidbodyContact contact)
		{
			var e = Mathf.Min(a.Restitution, b.Restitution);
			var friction = Mathf.Sqrt(a.Friction * b.Friction);

			var normal = contact.Normal;

			Vector3 relativeVelocity = a.GetVelocityAtPoint(contact.ContactPointA) - b.GetVelocityAtPoint(contact.ContactPointB);

			Vector3 rA = contact.ContactPointA - a.GetWorldCenterOfMass();
			Vector3 rB = contact.ContactPointB - b.GetWorldCenterOfMass();

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
	}
}