using Massive.ECS;

namespace Massive.Samples.Shooter
{
	public class BulletsUpdater : WorldUpdater
	{
		private View<BulletState> _bullets;

		public override void Init(MassiveRegistry registry)
		{
			_bullets = registry.View<BulletState>();
		}
		
		public override void UpdateWorld(float deltaTime)
		{
			foreach (var bullet in _bullets)
			{
				ref BulletState state = ref bullet.Get<BulletState>();
				
				state.Lifetime -= deltaTime;
				if (state.IsDestroyed)
				{
					bullet.Destroy();
					continue;
				}

				state.Transform.Position += state.Velocity * deltaTime;
			}
		}
	}
}