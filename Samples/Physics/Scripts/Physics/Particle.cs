using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using UnityEngine;

namespace Massive.Samples.Physics
{
	public struct Particle
	{
		public readonly float Radius;
		public readonly float Mass;
		public readonly float InverseMass;
		public readonly float Drag;

		public Vector3 LastPosition;
		public Vector3 Position;
		public Vector3 Velocity;
		public Vector3 Acceleration;

		public Vector3 PositionChange;
		public Vector3 VelocityChange;

		public Particle(Vector3 position, float radius, float mass = 1f, float drag = 1f)
		{
			Radius = radius;
			Mass = mass;
			InverseMass = 1f / Mass;
			Drag = drag;
			LastPosition = position;
			Position = position;
			Acceleration = Vector3.zero;
			Velocity = Vector3.zero;
			PositionChange = Vector3.zero;
			VelocityChange = Vector3.zero;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Integrate(float deltaTime)
		{
			float drag = (1f - deltaTime * Drag);
			Vector3 lastPosition  = LastPosition;
			
			LastPosition = Position;
			Position += (Position - lastPosition) * drag + VelocityChange * deltaTime + deltaTime * deltaTime * InverseMass * Acceleration;
			Velocity = (Position - LastPosition) / deltaTime;
			
			Acceleration = Vector3.zero;
			VelocityChange = Vector3.zero;
			PositionChange = Vector3.zero;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void AddMove(Vector3 delta)
		{
			Position += delta;
		}
		
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void AddAcceleration(Vector3 acceleration)
		{
			Acceleration += acceleration;
		}
		
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void AddVelocity(Vector3 velocity)
		{
			VelocityChange += velocity;
		}

		public static void IntegrateAll(in Frame<Particle> particles, float deltaTime)
		{
			var span = particles.GetAll();
			
			for (var i = 0; i < span.Length; i++)
			{
				span[i].Integrate(deltaTime);
			}
		}
	}
}