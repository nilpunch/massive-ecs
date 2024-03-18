namespace Massive.Samples.Shooter
{
	public class BulletsUpdater : WorldUpdater
	{
		private IRegistry _registry;
		private View<BulletState> _bullets;

		public override void Init(IRegistry registry)
		{
			_registry = registry;
			_bullets = new View<BulletState>(registry);
		}
		
		public override void UpdateWorld(float deltaTime)
		{
			_bullets.ForEachExtra(deltaTime, (int entity, ref BulletState state, float innerDeltaTime) =>
			{
				state.Lifetime -= innerDeltaTime;
				if (state.IsDestroyed)
				{
					_registry.Destroy(entity);
					return;
				}

				state.Transform.Position += state.Velocity * innerDeltaTime;
			});
		}
	}
}