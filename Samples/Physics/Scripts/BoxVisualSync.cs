using Massive.Samples.Shooter;
using UnityEngine;

namespace Massive.Samples.Physics
{
	[RequireComponent(typeof(BoxRoot))]
	public class BoxVisualSync : VisualSync<PhysicsBoxCollider>
	{
		public override void SyncState(ref PhysicsBoxCollider state)
		{
			base.SyncState(ref state);

			transform.localScale = state.Size;
		}

		protected override void TransformFromState(in PhysicsBoxCollider state, out EntityTransform transform)
		{
			transform = new EntityTransform() { Position = state.World.Position, Rotation = state.World.Rotation };
		}
	}
}
