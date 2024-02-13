using System.Runtime.CompilerServices;
using UnityEngine;

namespace MassiveData.Samples.Physics
{
	public struct Rigidbody
	{
		public Vector3 Position;
		public Vector3 Velocity;
		public Vector3 Forces;

		public Quaternion Rotation;
		public Vector3 AngularVelocity;
		public Vector3 Torques;

		public Vector3 CenterOfMass;
		public Vector3 InertiaTensor;
		public Vector3 InverseInertiaTensor;
		public float InverseMass;
		public float Mass;
		public float Restitution;
		public float Friction;

		public bool IsStatic;

		public Rigidbody(Vector3 position, Quaternion rotation, float mass, float restitution = 1f, float friction = 1f, float inertia = 1f, bool isStatic = false)
		{
			CenterOfMass = default;
			Mass = mass;
			InverseMass = 1f / mass;
			Restitution = restitution;
			Friction = friction;
			IsStatic = isStatic;

			Position = position;
			Rotation = rotation;
			Velocity = default;
			Forces = default;

			InertiaTensor = Vector3.one * inertia * mass; // Vector3.one * (2 / 5f * mass);
			InverseInertiaTensor = new Vector3(1f / InertiaTensor.x, 1f / InertiaTensor.y, 1f / InertiaTensor.z);
			AngularVelocity = default;
			Torques = default;
		}


		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Integrate(float deltaTime)
		{
			if (IsStatic)
			{
				Velocity = Vector3.zero;
				Forces = Vector3.zero;
				return;
			}
			
			// Linear motion integration
			Velocity += deltaTime * InverseMass * Forces;
			Position += deltaTime * Velocity;
			Forces = Vector3.zero;
			
			// Angular motion integration
			AngularVelocity += deltaTime * Vector3.Scale(InverseInertiaTensor, Torques);
			Rotation = Quaternion.Euler(deltaTime * Mathf.Rad2Deg * AngularVelocity) * Rotation;
			Torques = Vector3.zero;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void ApplyImpulseAtPoint(Vector3 impulse, Vector3 position)
		{
			if (!IsStatic)
			{
				// Apply linear impulse to the velocity
				Velocity += impulse * InverseMass;

				// Calculate the lever arm from the center of mass to the point of application
				Vector3 leverArm = position - GetWorldCenterOfMass();

				// Calculate the angular impulse using the cross product between the lever arm and the impulse
				Vector3 angularImpulse = Vector3.Cross(leverArm, impulse);

				// Apply angular impulse to the angular velocity
				AngularVelocity += Vector3.Scale(angularImpulse, InverseInertiaTensor);
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void ApplyForce(Vector3 force)
		{
			Forces += force * Mass;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Vector3 GetVelocityAtPoint(Vector3 point)
		{
			return Velocity + Vector3.Cross(AngularVelocity, point - GetWorldCenterOfMass());
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Vector3 GetWorldCenterOfMass()
		{
			return Position + Rotation * CenterOfMass;
		}

		public static void IntegrateAll(in Massive<Rigidbody> rigidbodies, float deltaTime)
		{
			var aliveRigidbodies = rigidbodies.AliveData;
			for (var i = 0; i < aliveRigidbodies.Length; i++)
			{
				aliveRigidbodies[i].Integrate(deltaTime);
			}
		}
	}
}