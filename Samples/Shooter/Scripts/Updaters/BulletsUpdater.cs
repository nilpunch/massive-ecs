using Massive.ECS;

namespace Massive.Samples.Shooter
{
	public class BulletsUpdater : WorldUpdater
	{
		private Registry _registry;
		private View<BulletState> _bullets;

		public override void Init(Registry registry)
		{
			_registry = registry;
			_bullets = registry.View<BulletState>();
		}
		
		public override void UpdateWorld(float deltaTime)
		{
			_bullets.ForEach((int id, ref BulletState state) =>
			{
				state.Lifetime -= deltaTime;
				if (state.IsDestroyed)
				{
					_registry.DeleteEntity(id);
					return;
				}

				state.Transform.Position += state.Velocity * deltaTime;
			});
		}
	}
}