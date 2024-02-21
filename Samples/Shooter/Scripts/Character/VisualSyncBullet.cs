namespace Massive.Samples.Shooter
{
	public class VisualSyncBullet : VisualSync<BulletState>
	{
		protected override void TransformFromState(in BulletState state, out EntityTransform transform)
		{
			transform = state.Transform;
		}
	}
}