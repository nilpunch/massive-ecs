namespace Massive.Samples.Shooter
{
	/// <summary>
	/// Just delays a full destruction of a bullet to apply some visual death effect.
	/// </summary>
	public static class DelayedBulletDeathSystem
	{
		public static void Update(IRegistry registry, float deltaTime)
		{
			registry.View().Include<Bullet>().ForEachExtra((registry, deltaTime),
				(int id, ref Dead dead, (IRegistry Registry, float DeltaTime) args) =>
				{
					dead.ElapsedTimeSinceDeath += args.DeltaTime;

					if (dead.ElapsedTimeSinceDeath > 1f)
					{
						args.Registry.Destroy(id);
					}
				});
		}
	}
}
