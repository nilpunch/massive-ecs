using System;
using UnityEngine;

namespace Massive.Samples.Physics
{
	public static class Collisions
	{
		public static void Solve(in MassiveData<PointMass> particles, float beta = 0.75f)
		{
			var span = particles.Data;
			var aliveCount = particles.AliveCount;
			for (int i = 0; i < aliveCount; ++i)
			{
				ref PointMass a = ref span[i];

				for (int j = i + 1; j < aliveCount; ++j)
				{
					ref PointMass b = ref span[j];
					Vector3 axis = a.Position - b.Position;
					float sqrDistance = axis.sqrMagnitude;
					float minDistance = 0f; // a.Radius + b.Radius;

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