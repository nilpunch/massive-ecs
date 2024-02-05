using System;
using UnityEngine;

namespace Massive.Samples.Physics
{
	public static class GlobalFloor
	{
		public static void Apply(in MassiveData<PointMass> particles, float height = 0f, float frictionCoefficient = 0.8f)
		{
			var span = particles.Data;
			var aliveCount = particles.AliveCount;
			for (var i = 0; i < aliveCount; i++)
			{
				ref PointMass pointMass = ref span[i];

				if (pointMass.Position.y < height)
				{
					pointMass.Position.y = height;

					// Simulate friction as a kinetic energy loss
					pointMass.AddVelocity(-frictionCoefficient * Vector3.ProjectOnPlane(pointMass.Velocity, Vector3.up));
				}
			}
		}
	}
}