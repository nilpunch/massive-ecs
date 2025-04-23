namespace Massive.Samples.Shooter
{
	/// <summary>
	/// Just delays a full destruction of a bullet to apply some visual death effect.
	/// </summary>
	public static class DelayedBulletDeathSystem
	{
		public static void Update(World world, float deltaTime)
		{
			world.View().Include<Bullet>().ForEach((world, deltaTime),
				static (int id, ref Dead dead, (World World, float DeltaTime) args) =>
				{
					var (world, deltaTime) = args;

					dead.ElapsedTimeSinceDeath += deltaTime;

					if (dead.ElapsedTimeSinceDeath > 1f)
					{
						world.Destroy(id);
					}
				});
		}
	}
}
