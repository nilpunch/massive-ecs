using MassiveData.Samples.Shooter;
using UnityEngine;

namespace MassiveData.Samples.Physics
{
	[RequireComponent(typeof(SphereRoot))]
	public class SphereVisualSync : VisualSync<SphereCollider>
	{
		public override void SyncState(ref SphereCollider state)
		{
			base.SyncState(ref state);

			transform.localScale = Vector3.one * state.Radius;
		}

		protected override void TransformFromState(in SphereCollider state, out EntityTransform transform)
		{
			transform = new EntityTransform() { Position = state.World.Position, Rotation = state.World.Rotation };
		}
	}
}