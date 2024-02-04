using UnityEngine;

namespace Massive.Samples.Physics
{
	public static class GlobalFloorConstraint
	{
		public static void Apply(in Frame<Particle> particles, float height = 0f, float frictionCoefficient = 0.8f)
		{
			var span = particles.GetAll();
			for (var i = 0; i < span.Length; i++)
			{
				ref Particle particle = ref span[i];

				if (particle.Position.y < particle.Radius + height)
				{
					particle.Position.y = particle.Radius + height;
					
					// Simulate friction as a kinetic energy loss
					particle.AddVelocity(-frictionCoefficient * particle.Velocity);
				}
			}
		}
	}
}