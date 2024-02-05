using Massive.Samples.Shooter;
using UnityEngine;

namespace Massive.Samples.Physics
{
	public class ParticleVisualSync : VisualSync<PointMass>
	{
		public override void SyncState(ref PointMass state)
		{
			base.SyncState(ref state);

			transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
		}

		protected override void TransformFromState(in PointMass state, out EntityTransform transform)
		{
			transform = new EntityTransform() { Position = state.Position, Rotation = Quaternion.identity };
		}
	}
}