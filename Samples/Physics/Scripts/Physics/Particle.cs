using System.Runtime.CompilerServices;
using UnityEngine;

namespace Massive.Samples.Physics
{
	public struct Particle
	{
		public readonly float Radius;
		public readonly float Mass;
		public readonly float InverseMass;

		public Vector3 LastPosition;
		public Vector3 Position;
		public Vector3 Velocity;
		public Vector3 Acceleration;

		public Particle(Vector3 position, float radius, float mass = 1f)
		{
			Radius = radius;
			Mass = mass;
			InverseMass = 1f / Mass;
			LastPosition = position;
			Position = position;
			Velocity = Vector3.zero;
			Acceleration = Vector3.zero;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Integrate(float deltaTime)
		{
			Vector3 displacement = Position - LastPosition;
			
			LastPosition = Position;
			
			Position += displacement + deltaTime * deltaTime * InverseMass * Acceleration;
			Acceleration = Vector3.zero;
			
			// Not used in calculations
			Velocity = displacement / deltaTime;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void MoveDelta(Vector3 delta)
		{
			Position += delta;
		}
		
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Accelerate(Vector3 acceleration)
		{
			Acceleration += acceleration;
		}

		public static void Update(in Frame<Particle> particles, float deltaTime)
		{
			var span = particles.GetAll();
			for (var i = 0; i < span.Length; i++)
			{
				span[i].Integrate(deltaTime);
			}
		}
	}
}