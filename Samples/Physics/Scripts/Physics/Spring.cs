using UnityEngine;

namespace Massive.Samples.Physics
{
	public struct Spring
	{
		public readonly int ParticleA;
		public readonly int ParticleB;

		public readonly float TargetDistance;
		public readonly float Strength;
		public readonly float MaxElongationRatio;

		public bool Broken;

		public Spring(int a, int b, float targetDistance = 0f, float strength = 1f, float maxElongationRatio = 1.5f)
		{
			ParticleA = a;
			ParticleB = b;
			TargetDistance = targetDistance;
			Strength = strength;
			MaxElongationRatio = maxElongationRatio;
			Broken = false;
		}
		
		public void Apply(in Frame<Particle> particles)
		{
			if (Broken)
			{
				return;
			}
			
			Particle a = particles.Get(ParticleA);
			Particle b = particles.Get(ParticleB);
			Vector3 axis = a.Position - b.Position;
			float distance = axis.magnitude;
			Vector3 normal = axis / distance;
			float delta = TargetDistance - distance;

			Broken = distance > TargetDistance * MaxElongationRatio;
			
			Vector3 displacement = -delta * Strength / (a.Mass + b.Mass) * normal;
			
			a.MoveDelta(-displacement * a.InverseMass);
			a.MoveDelta(displacement * b.InverseMass);
		}
	}
}