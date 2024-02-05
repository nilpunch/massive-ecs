using System;
using UnityEngine;

namespace Massive.Samples.Physics
{
	public struct Spring
	{
		public readonly int ParticleA;
		public readonly int ParticleB;

		public readonly float Strength;
		public readonly float MaxElongationRatio;
		public readonly float RestLength;

		public bool Broken;

		public Spring(int a, int b, float restLength = 0f, float strength = 1f, float maxElongationRatio = 1.5f)
		{
			ParticleA = a;
			ParticleB = b;
			RestLength = restLength;
			Strength = strength;
			MaxElongationRatio = maxElongationRatio;
			Broken = false;
		}

		public void Apply(in MassiveData<PointMass> particles, float deltaTime)
		{
			if (Broken)
			{
				return;
			}

			ref PointMass a = ref particles.Get(ParticleA);
			ref PointMass b = ref particles.Get(ParticleB);
			Vector3 displacement = a.Position - b.Position;
			float distance = displacement.magnitude;
			Vector3 normal = displacement / distance;
			float delta = RestLength - distance;

			Broken = distance > RestLength * MaxElongationRatio;

			Vector3 movement = -delta * Strength * normal * deltaTime;

			a.AddMove(-movement * a.InverseMass);
			b.AddMove(movement * b.InverseMass);
		}

		public static void ApplyAll(in MassiveData<Spring> springsFrame, in MassiveData<PointMass> particlesFrame, float deltaTime)
		{
			var springs = springsFrame.Data;
			for (var dense = 0; dense < springsFrame.AliveCount; dense++)
			{
				springs[dense].Apply(particlesFrame, deltaTime);

				if (springs[dense].Broken)
				{
					springsFrame.DeleteDense(dense);
					dense -= 1;
				}
			}
		}
	}
}