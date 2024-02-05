using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using UnityEngine;

namespace Massive.Samples.Physics
{
	public struct PointMass
	{
		public readonly int SoftBodyId;
		public Vector3 LocalReferencePosition;
		public float ReferenceSpring;
		
		public readonly float Mass;
		public readonly float InverseMass;
		public readonly float Drag;

		public bool Immovable;
		
		public Vector3 LastPosition;
		public Vector3 Position;
		public Vector3 Velocity;
		public Vector3 Acceleration;

		public Vector3 VelocityChange;

		public PointMass(int softBodyId,
			Vector3 position,
			float mass = 1f,
			float drag = 1f,
			bool immovable = false,
			Vector3 localReferencePosition = default,
			float referenceSpring = 0f)
		{
			SoftBodyId = softBodyId;
			LocalReferencePosition = localReferencePosition;
			ReferenceSpring = referenceSpring;
			
			Mass = mass;
			InverseMass = 1f / Mass;
			Drag = drag;
			Immovable = immovable;

			LastPosition = position;
			Position = position;
			Acceleration = Vector3.zero;
			Velocity = Vector3.zero;
			VelocityChange = Vector3.zero;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Integrate(float deltaTime)
		{
			if (Immovable)
			{
				LastPosition = Position;
				Acceleration = Vector3.zero;
				VelocityChange = Vector3.zero;
				return;
			}
			
			float drag = 1f - deltaTime * Drag;
			Vector3 lastPosition = LastPosition;

			LastPosition = Position;
			Position += (Position - lastPosition) * drag + deltaTime * (VelocityChange + deltaTime * InverseMass * Acceleration);
			Velocity = (Position - LastPosition) / deltaTime;

			Acceleration = Vector3.zero;
			VelocityChange = Vector3.zero;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Teleport(Vector3 position)
		{
			Position = position;
			LastPosition = position;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void AddMove(Vector3 delta)
		{
			if (!Immovable)
			{
				Position += delta;
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void AddVelocity(Vector3 velocity)
		{
			VelocityChange += velocity;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void AddForce(Vector3 acceleration)
		{
			Acceleration += acceleration;
		}

		public static void IntegrateAll(in MassiveData<PointMass> particles, float deltaTime)
		{
			var span = particles.Data;
			var aliveCount = particles.AliveCount;

			for (var i = 0; i < aliveCount; i++)
			{
				span[i].Integrate(deltaTime);
			}
		}
	}
}