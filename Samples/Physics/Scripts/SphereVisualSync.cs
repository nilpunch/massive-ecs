using Massive.Samples.Shooter;
using UnityEngine;

namespace Massive.Samples.Physics
{
	[RequireComponent(typeof(SphereRoot))]
	public class SphereVisualSync : VisualSync<PhysicsSphereCollider>
	{
		public override void SyncState(ref PhysicsSphereCollider state)
		{
			base.SyncState(ref state);

			transform.localScale = Vector3.one * state.Radius;
		}

		protected override void TransformFromState(in PhysicsSphereCollider state, out EntityTransform transform)
		{
			transform = new EntityTransform() { Position = state.World.Position, Rotation = state.World.Rotation };
		}
	}
}
