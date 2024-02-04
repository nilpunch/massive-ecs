using System;
using UnityEngine;

namespace Massive.Samples.Physics
{
	public static class GlobalFloorConstraint
	{
		public static void Apply(in MassiveData<Particle> particles, float height = 0f, float frictionCoefficient = 0.8f)
		{
			var span = particles.Data;
			var aliveCount = particles.AliveCount;
			for (var i = 0; i < aliveCount; i++)
			{
				ref Particle particle = ref span[i];

				if (particle.Position.y < particle.Radius + height)
				{
					particle.Position.y = particle.Radius + height;

					// Simulate friction as a kinetic energy loss
					particle.AddVelocity(-frictionCoefficient * Vector3.ProjectOnPlane(particle.Velocity, Vector3.up));
				}
			}
		}
	}
}