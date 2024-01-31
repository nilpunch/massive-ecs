using System.Linq;
using UnityEngine;

namespace Massive.Samples.Shooter
{
	public class UpdateRoot<TState> : MonoBehaviour, IUpdateComponent<TState> where TState : struct
	{
		private IUpdateComponent<TState>[] _components;

		private void Awake()
		{
			_components = GetComponentsInChildren<IUpdateComponent<TState>>().Where(component => !ReferenceEquals(component, this)).ToArray();
		}

		public void UpdateState(World world, ref TState state)
		{
			foreach (var component in _components)
			{
				component.UpdateState(world, ref state);
			}
		}
	}
}