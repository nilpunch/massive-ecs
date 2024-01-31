using UnityEngine;

namespace Massive.Samples.Shooter
{
	public class BulletsUpdater : WorldUpdater
	{
		public override void UpdateWorld(in WorldFrame worldFrame)
		{
			var ids = worldFrame.Bullets.GetAllIds();
			var bullets = worldFrame.Bullets.GetAll();

			for (int i = 0; i < worldFrame.Bullets.AliveCount; i++)
			{
				ref BulletState state = ref bullets[i];
				
				state.Lifetime -= worldFrame.DeltaTime;
				if (state.IsDestroyed)
				{
					worldFrame.Bullets.Delete(ids[i]);
					continue;
				}
				
				state.Transform.Position += state.Velocity * worldFrame.DeltaTime;
			}
		}
	}
}