namespace Massive.Samples.Shooter
{
	public class BulletLifetimeSystem : SystemBase, IUpdate
	{
		public void Update(float deltaTime)
		{
			World.Exclude<Dead>().ForEach((World, deltaTime),
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
