namespace Massive.Samples.Shooter
{
	public class BulletsUpdater : WorldUpdater
	{
		public override void UpdateWorld(in WorldFrame worldFrame)
		{
			var bullets = worldFrame.Bullets.AliveData;

			for (int dense = worldFrame.Bullets.AliveCount - 1; dense >= 0; dense--)
			{
				ref BulletState state = ref bullets[dense];

				state.Lifetime -= worldFrame.DeltaTime;
				if (state.IsDestroyed)
				{
					worldFrame.Bullets.DeleteDense(dense);
					continue;
				}

				state.Transform.Position += state.Velocity * worldFrame.DeltaTime;
			}
		}
	}
}