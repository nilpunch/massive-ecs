using System.Linq;
using UnityEngine;

namespace Massive.Samples.Shooter
{
	public class EntityRoot<TState> : MonoBehaviour, IWorldEntity<TState> where TState : struct
	{
		private IWorldComponent<TState>[] _components;
		private Renderer[] _renderers;

		private void Awake()
		{
			_components = GetComponentsInChildren<IWorldComponent<TState>>().Where(component => !ReferenceEquals(component, this)).ToArray();

			_renderers = GetComponentsInChildren<Renderer>();
		}

		public void UpdateState(World world, ref TState state)
		{
			foreach (var component in _components)
			{
				component.UpdateState(world, ref state);
			}
		}

		public void Enable()
		{
			foreach (var renderer in _renderers)
			{
				renderer.enabled = true;
			}
		}

		public void Disable()
		{
			foreach (var renderer in _renderers)
			{
				renderer.enabled = false;
			}
		}
	}
}