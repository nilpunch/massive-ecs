namespace Massive.Samples.UpdateLoop
{
	public class HealingBuffSystem : WorldSystem, IUpdate
	{
		public void Update(float deltaTime)
		{
			World.Include<HealingBuff>().ForEach((ref Health health) =>
			{
				health.Value += 1;
			});
		}
	}
}
