using UnityEngine;

namespace Massive.Samples.Physics
{
	public static class Collisions
	{
		public static void Solve(in Frame<Particle> particles, float beta = 0.75f)
		{
			var span = particles.GetAll();
			for (int i = 0; i < span.Length; ++i)
			{
				ref Particle a = ref span[i];

				for (int j = i + 1; j < span.Length; ++j)
				{
					ref Particle b = ref span[j];
					Vector3 axis = a.Position - b.Position;
					float sqrDistance = axis.sqrMagnitude;
					float minDistance = a.Radius + b.Radius;

					if (sqrDistance < minDistance * minDistance)
					{
						float distance = Mathf.Sqrt(sqrDistance);
						Vector3 normal = axis / distance;
						float delta = 0.5f * (distance - minDistance) * beta;
						float systemMass = a.Mass + b.Mass;

						a.Position -= delta * systemMass * a.InverseMass * normal;
						b.Position += delta * systemMass * a.InverseMass * normal;
					}
				}
			}
		}
	}
}