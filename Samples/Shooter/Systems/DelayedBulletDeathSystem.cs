namespace Massive.Samples.Shooter
{
	/// <summary>
	/// Just delays a full destruction of a bullet to apply some visual death effect.
	/// </summary>
	public static class DelayedBulletDeathSystem
	{
		public static void Update(Registry registry, float deltaTime)
		{
			registry.View().Include<Bullet>().ForEachExtra((registry, deltaTime),
				static (int id, ref Dead dead, (Registry Registry, float DeltaTime) args) =>
				{
					var (registry, deltaTime) = args;

					dead.ElapsedTimeSinceDeath += deltaTime;

					if (dead.ElapsedTimeSinceDeath > 1f)
					{
						registry.Destroy(id);
					}
				});
		}
	}
}
