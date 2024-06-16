namespace Massive.Samples.Shooter
{
	public static class BulletLifetimeSystem
	{
		public static void Update(IRegistry registry, float deltaTime)
		{
			registry.View().Exclude<Dead>().ForEachExtra((registry, deltaTime),
				(int bulletId, ref Bullet bullet, (IRegistry Registry, float DeltaTime) args) =>
				{
					bullet.Lifetime -= args.DeltaTime;

					if (bullet.Lifetime <= 0f)
					{
						args.Registry.Assign<Dead>(bulletId);
					}
				});
		}
	}
}
