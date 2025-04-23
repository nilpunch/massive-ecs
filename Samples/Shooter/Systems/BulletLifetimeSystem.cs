namespace Massive.Samples.Shooter
{
	public static class BulletLifetimeSystem
	{
		public static void Update(World world, float deltaTime)
		{
			world.View().Exclude<Dead>().ForEach((world, deltaTime),
				static (int bulletId, ref Bullet bullet, (World World, float DeltaTime) args) =>
				{
					var (world, deltaTime) = args;

					bullet.Lifetime -= deltaTime;

					if (bullet.Lifetime <= 0f)
					{
						world.Add<Dead>(bulletId);
					}
				});
		}
	}
}
