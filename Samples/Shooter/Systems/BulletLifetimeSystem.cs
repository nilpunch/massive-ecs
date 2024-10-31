namespace Massive.Samples.Shooter
{
	public static class BulletLifetimeSystem
	{
		public static void Update(Registry registry, float deltaTime)
		{
			foreach (var bulletId in registry.View().Filter<Include<Bullet>, Exclude<Dead>>())
			{
				ref var bullet = ref registry.Get<Bullet>(bulletId);

				bullet.Lifetime -= deltaTime;

				if (bullet.Lifetime <= 0f)
				{
					registry.Assign<Dead>(bulletId);
				}
			}
		}
	}
}
