using System.Runtime.CompilerServices;
using UnityEngine;

namespace MassiveData.Samples.Physics
{
	public struct Rigidbody
	{
		public Vector3 Position;
		public Vector3 Velocity;
		public Vector3 Forces;

		public float InverseMass;
		public float Mass;
		public float Restitution;

		public bool IsStatic;

		public Rigidbody(Vector3 position, float mass, float restitution = 1f, bool isStatic = false)
		{
			Position = position;
			Mass = mass;
			InverseMass = 1f / mass;
			Restitution = restitution;

			Velocity = Vector3.zero;
			Forces = Vector3.zero;
			IsStatic = isStatic;
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
			
			Velocity += deltaTime * InverseMass * Forces;
			Position += deltaTime * Velocity;
			Forces = Vector3.zero;
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