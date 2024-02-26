using Massive.ECS;

namespace Massive.Samples.Shooter
{
	public class BulletsUpdater : WorldUpdater
	{
		private View<BulletState> _bullets;

		public override void Init(Registry registry)
		{
			_bullets = registry.View<BulletState>();
		}
		
		public override void UpdateWorld(float deltaTime)
		{
			_bullets.ForEach((Entity entity, ref BulletState state) =>
			{
				state.Lifetime -= deltaTime;
				if (state.IsDestroyed)
				{
					entity.Destroy();
					return;
				}

				state.Transform.Position += state.Velocity * deltaTime;
			});
		}
	}
}