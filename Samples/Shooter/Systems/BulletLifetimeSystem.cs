﻿namespace Massive.Samples.Shooter
{
	public static class BulletLifetimeSystem
	{
		public static void Update(Registry registry, float deltaTime)
		{
			registry.View().Exclude<Dead>().ForEachExtra((registry, deltaTime),
				static (int bulletId, ref Bullet bullet, (Registry Registry, float DeltaTime) args) =>
				{
					var (registry, deltaTime) = args;

					bullet.Lifetime -= deltaTime;

					if (bullet.Lifetime <= 0f)
					{
						registry.Assign<Dead>(bulletId);
					}
				});
		}
	}
}
