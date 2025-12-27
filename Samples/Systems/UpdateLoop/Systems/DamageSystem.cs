namespace Massive.Samples.UpdateLoop
{
	public class DamageSystem : WorldSystem, IUpdate, IInitinalize
	{
		private BitSet TakeDamageSelf;

		public void Initialize()
		{
			TakeDamageSelf = World.BitSet<TakeDamageSelf>();
		}

		public void Update(float deltaTime)
		{
			World.ForEach(this, (int entityId, ref Health health, ref TakeDamageSelf damageSelf, DamageSystem system) =>
			{
				health.Value -= damageSelf.Value;
				system.TakeDamageSelf.Remove(entityId);
			});
		}
	}
}
