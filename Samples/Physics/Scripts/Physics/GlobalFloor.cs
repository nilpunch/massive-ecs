using UnityEngine;

namespace Massive.Samples.Physics
{
	public static class GlobalFloor
	{
		public static void Apply(in Frame<Particle> particles, float height = 0f)
		{
			var span = particles.GetAll();
			for (var i = 0; i < span.Length; i++)
			{
				ref Particle particle = ref span[i];
				particle.Position.y = Mathf.Max(particle.Position.y, particle.Radius + height);
			}
		}
	}
}