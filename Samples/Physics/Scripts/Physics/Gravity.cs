using System;
using UnityEngine;

namespace Massive.Samples.Physics
{
	public static class Gravity
	{
		public static void Apply(in MassiveData<PointMass> particles, float gravity = 10f)
		{
			var data = particles.Data;
			var aliveCount = particles.AliveCount;
			for (var i = 0; i < aliveCount; i++)
			{
				data[i].AddForce(Vector3.down * gravity * data[i].Mass);
			}
		}
	}
}