using Massive.ECS;

namespace Massive.Samples.Shooter
{
	public class BulletsUpdater : WorldUpdater
	{
		private IRegistry _registry;
		private IDataSet<BulletState> _bullets;

		public override void Init(IRegistry registry)
		{
			_registry = registry;
			_bullets = _registry.Components<BulletState>();
		}
		
		public override void UpdateWorld(float deltaTime)
		{
			var aliveData = _bullets.AliveData;
			var aliveIds = _bullets.AliveIds;

			for (int i = aliveData.Length - 1; i >= 0; i--)
			{
				ref BulletState state = ref aliveData[i];
				
				state.Lifetime -= deltaTime;
				if (state.IsDestroyed)
				{
					_registry.Destroy(aliveIds[i]);
					continue;
				}

				state.Transform.Position += state.Velocity * deltaTime;
			}
		}
	}
}