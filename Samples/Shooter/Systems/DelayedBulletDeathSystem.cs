namespace Massive.Samples.Shooter
{
	/// <summary>
	/// Just delays a full destruction of a bullet to apply some visual death effect.
	/// </summary>
	public static class DelayedBulletDeathSystem
	{
		public static void Update(Registry registry, float deltaTime)
		{
			foreach (var bulletId in registry.View().Filter<Include<Dead, Bullet>>())
			{
				ref var dead = ref registry.Get<Dead>(bulletId);
				
				dead.ElapsedTimeSinceDeath += deltaTime;

				if (dead.ElapsedTimeSinceDeath > 1f)
				{
					registry.Destroy(bulletId);
				}
			}
		}
	}
}
