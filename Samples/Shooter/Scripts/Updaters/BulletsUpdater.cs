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
			_bullets.ForEachExtra((_registry, deltaTime), (int entityId, ref BulletState state, (IRegistry Registry, float DeltaTime) inner) =>
			{
				state.Lifetime -= inner.DeltaTime;
				if (state.IsDestroyed)
				{
					inner.Registry.Destroy(entityId);
					return;
				}

				state.Transform.Position += state.Velocity * inner.DeltaTime;
			});
		}
	}
}
