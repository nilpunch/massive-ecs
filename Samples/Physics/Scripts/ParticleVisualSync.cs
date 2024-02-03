using Massive.Samples.Shooter;
using UnityEngine;

namespace Massive.Samples.Physics
{
	public class ParticleVisualSync : VisualSync<Particle>
	{
		public override void SyncState(ref Particle state)
		{
			base.SyncState(ref state);

			transform.localScale = new Vector3(state.Radius, state.Radius, state.Radius);
		}

		protected override void TransformFromState(in Particle state, out EntityTransform transform)
		{
			transform = new EntityTransform() { Position = state.Position, Rotation = Quaternion.identity};
		}
	}
}