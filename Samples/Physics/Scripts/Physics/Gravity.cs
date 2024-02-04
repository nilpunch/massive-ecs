using System;
using UnityEngine;

namespace Massive.Samples.Physics
{
	public static class Gravity
	{
		public static void Apply(in MassiveData<Particle> particles, float gravity = 10f)
		{
			var span = particles.Data;
			var aliveCount = particles.AliveCount;
			for (var i = 0; i < aliveCount; i++)
			{
				span[i].AddAcceleration(Vector3.down * gravity);
			}
		}
	}
}