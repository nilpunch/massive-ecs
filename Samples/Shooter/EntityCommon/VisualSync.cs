using UnityEngine;

namespace Massive.Samples.Shooter
{
	public abstract class VisualSync<TState> : MonoBehaviour, IWorldComponent<TState>
	{
		private EntityTransform _entityTransform;

		public void UpdateState(World world, ref TState state)
		{
			TransformFromState(in state, out _entityTransform);
		}

		private void LateUpdate()
		{
			transform.position = _entityTransform.Position;
			transform.rotation = _entityTransform.Rotation;
		}

		protected abstract void TransformFromState(in TState state, out EntityTransform transform);
	}
}