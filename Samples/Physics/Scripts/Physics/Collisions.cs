using System;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

namespace MassiveData.Samples.Physics
{
	public struct RigidbodyContact
	{
		public int BodyA;
		public int BodyB;

		public Vector3 ContactPoint;
		public Vector3 Normal;
		public float Depth;
	}

	public struct ColliderContact
	{
		public Vector3 OffsetFromColliderA;
		public Vector3 Normal;
		public float Depth;
	}

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
				ref SphereCollider a = ref aliveSpheres[i];
				for (int j = i + 1; j < aliveSpheres.Length; ++j)
				{
					ref SphereCollider b = ref aliveSpheres[j];

					if (a.RigidbodyId == b.RigidbodyId || bodies.Get(a.RigidbodyId).IsStatic && bodies.Get(b.RigidbodyId).IsStatic)
					{
						continue;
					}

					var contact = new ColliderContact();
					CollisionTester.SphereVsSphere(ref a, ref b, b.WorldPosition - a.WorldPosition, ref contact);

					if (contact.Depth > 0f)
					{
						Contacts.Add(new RigidbodyContact()
						{
							BodyA = a.RigidbodyId, BodyB = b.RigidbodyId,
							Normal = contact.Normal,
							Depth = contact.Depth,
							ContactPoint = a.TransformFromLocalToWorld(contact.OffsetFromColliderA)
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
						Contacts.Add(new RigidbodyContact()
						{
							BodyA = a.RigidbodyId, BodyB = b.RigidbodyId,
							Normal = contact.Normal,
							Depth = contact.Depth,
							ContactPoint = a.TransformFromLocalToWorld(contact.OffsetFromColliderA)
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

			var contactPoint = contact.ContactPoint;
			var normal = contact.Normal;
			
			Vector3 relativeVelocity = a.GetVelocityAtPoint(contactPoint) - b.GetVelocityAtPoint(contactPoint);
			
			Vector3 rA = contactPoint - a.GetWorldCenterOfMass();
			Vector3 rB = contactPoint - b.GetWorldCenterOfMass();
			
			Vector3 raCrossN = Vector3.Cross(rA, normal);
			Vector3 rbCrossN = Vector3.Cross(rB, normal);

			float angularInertiaA = a.IsStatic ? 0f : Vector3.Dot(raCrossN, Vector3.Scale(a.InverseInertiaTensor, raCrossN));
			float angularInertiaB = b.IsStatic ? 0f : Vector3.Dot(rbCrossN, Vector3.Scale(b.InverseInertiaTensor, rbCrossN));
			float angularInertia = angularInertiaA + angularInertiaB;
			
			float inverseMassA = a.IsStatic ? 0f : a.InverseMass;
			float inverseMassB = b.IsStatic ? 0f : b.InverseMass;
			float inverseMassSum = inverseMassA + inverseMassB;

			// Calculate normal impulse scalar
			float j = -(1 + e) * Vector3.Dot(relativeVelocity, normal) / (inverseMassSum + angularInertia);

			// Ignore separation impulse
			if (j < 0f)
			{
				return;
			}
			
			// Apply normal impulse
			Vector3 impulse = normal * j;
			a.ApplyImpulseAtPoint(impulse, contactPoint);
			b.ApplyImpulseAtPoint(-impulse, contactPoint);
			
			// Recalculate relative velocity after normal impulse for friction calculations
			relativeVelocity = a.GetVelocityAtPoint(contactPoint) - b.GetVelocityAtPoint(contactPoint);

			Vector3 tangent = default;
			Vector3 biTangent = default;
			Vector3.OrthoNormalize(ref normal, ref tangent, ref biTangent);
			
			float jt = -Vector3.Dot(relativeVelocity, tangent) / (inverseMassSum + angularInertia);
			float jbt = -Vector3.Dot(relativeVelocity, biTangent) / (inverseMassSum + angularInertia);

			// Correctly clamp the friction impulse magnitude based on the normal impulse
			float maxFrictionImpulse = j * friction;
			float tangentImpulseMagnitude = Mathf.Clamp(jt, -maxFrictionImpulse, maxFrictionImpulse);
			float biTangentImpulseMagnitude = Mathf.Clamp(jbt, -maxFrictionImpulse, maxFrictionImpulse);
			
			// Apply friction impulse
			Vector3 frictionImpulse = tangent * tangentImpulseMagnitude + biTangent * biTangentImpulseMagnitude;
				
			a.ApplyImpulseAtPoint(frictionImpulse, contactPoint);
			b.ApplyImpulseAtPoint(-frictionImpulse, contactPoint);
		}
	}
}