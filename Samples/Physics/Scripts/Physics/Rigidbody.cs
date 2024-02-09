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
		public Vector3 InverseInertiaTensor;
		public float InverseMass;
		public float Mass;
		public float Restitution;

		public bool IsStatic;

		public Rigidbody(Vector3 position, Quaternion rotation, float mass, float restitution = 1f, bool isStatic = false)
		{
			CenterOfMass = default;
			Mass = mass;
			InverseMass = 1f / mass;
			Restitution = restitution;
			IsStatic = isStatic;

			Position = position;
			Rotation = rotation;
			Velocity = default;
			Forces = default;

			InverseInertiaTensor = Vector3.one * (5f / (2 * mass * 1f));
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
			
			// // Angular motion integration
			// AngularVelocity += deltaTime * Vector3.Scale(Torques, InverseInertiaTensor);
			// Rotation = Rotation * Quaternion.Euler(deltaTime * Mathf.Rad2Deg * AngularVelocity);
			// Torques = Vector3.zero;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void ApplyImpulse(Vector3 impulse)
		{
			if (!IsStatic)
			{
				Velocity += impulse;
			}
		}
		
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void ApplyForce(Vector3 force)
		{
			Forces += force;
		}
		
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void ApplyAngularImpulse(Vector3 impulse)
		{
			if (!IsStatic)
			{
				AngularVelocity += impulse;
			}
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