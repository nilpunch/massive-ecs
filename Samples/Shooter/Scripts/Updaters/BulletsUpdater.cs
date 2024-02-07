using System;
using UnityEngine;

namespace MassiveData.Samples.Shooter
{
	public class BulletsUpdater : WorldUpdater
	{
		public override void UpdateWorld(in WorldFrame worldFrame)
		{
			var bullets = worldFrame.Bullets.AliveData;

			for (int dense = 0; dense < worldFrame.Bullets.AliveCount; dense++)
			{
				ref BulletState state = ref bullets[dense];

				state.Lifetime -= worldFrame.DeltaTime;
				if (state.IsDestroyed)
				{
					worldFrame.Bullets.DeleteDense(dense);
					dense -= 1;
					continue;
				}

				state.Transform.Position += state.Velocity * worldFrame.DeltaTime;
			}
		}
	}
}