using System.Linq;
using UnityEngine;

namespace Massive.Samples.Shooter
{
	public class EntityRoot<TState> : MonoBehaviour, ISyncEntity<TState> where TState : unmanaged
	{
		private ISyncComponent<TState>[] _components;
		private Renderer[] _renderers;

		private void Awake()
		{
			_components = GetComponentsInChildren<ISyncComponent<TState>>().Where(component => !ReferenceEquals(component, this)).ToArray();

			_renderers = GetComponentsInChildren<Renderer>();
		}

		public void SyncState(ref TState state)
		{
			foreach (var component in _components)
			{
				component.SyncState(ref state);
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
