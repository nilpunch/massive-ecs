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
		
		public void Apply(in Frame<Particle> particles, float deltaTime)
		{
			if (Broken)
			{
				return;
			}
			
			ref Particle a = ref particles.GetFast(ParticleA);
			ref Particle b = ref particles.GetFast(ParticleB);
			Vector3 axis = a.Position - b.Position;
			float distance = axis.magnitude;
			Vector3 normal = axis / distance;
			float delta = RestLength - distance;

			Broken = distance > RestLength * MaxElongationRatio;
			
			Vector3 displacement = -delta * Strength / (a.Mass + b.Mass) * normal;
			
			a.AddMove(deltaTime * a.InverseMass * -displacement);
			b.AddMove(deltaTime * b.InverseMass * displacement);
		}
		
		public static void ApplyAll(in Frame<Spring> springsFrame, in Frame<Particle> particlesFrame, float deltaTime)
		{
			var springs = springsFrame.GetAll();
			for (var dense = 0; dense < springs.Length; dense++)
			{
				springs[dense].Apply(particlesFrame, deltaTime);
			}
			
			for (var dense = 0; dense < springsFrame.AliveCount; dense++)
			{
				if (springs[dense].Broken)
				{
					springsFrame.DeleteDense(dense);
					dense -= 1;
				}
			}
		}
	}
}