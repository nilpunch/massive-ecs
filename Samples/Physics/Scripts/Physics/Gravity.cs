using UnityEngine;

namespace Massive.Samples.Physics
{
	public static class Gravity
	{
		public static void Apply(in Frame<Particle> particles, float gravity = 10f)
		{
			var span = particles.GetAll();
			for (var i = 0; i < span.Length; i++)
			{
				span[i].AddAcceleration(Vector3.down * gravity);
			}
		}
	}
}