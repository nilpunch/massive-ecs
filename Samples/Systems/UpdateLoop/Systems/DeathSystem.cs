namespace Massive.Samples.UpdateLoop
{
	public class DeathSystem : WorldSystem, IUpdate
	{
		public void Update(float deltaTime)
		{
			World.ForEach((Entity entity, ref Health health) =>
			{
				if (health.Value <= 0)
				{
					entity.Destroy();
				}
			});
		}
	}
}
