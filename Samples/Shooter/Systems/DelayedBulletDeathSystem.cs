namespace Massive.Samples.Shooter
{
	/// <summary>
	/// Just delays a full destruction of a bullet to apply some visual death effect.
	/// </summary>
	public class DelayedBulletDeathSystem : WorldSystem, IUpdate
	{
		public void Update(float deltaTime)
		{
			World.Include<Bullet>().ForEach((World, deltaTime),
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
