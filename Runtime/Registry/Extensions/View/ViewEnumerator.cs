using System;

namespace Massive.ECS
{
	public ref struct ViewEnumerator
	{
		private readonly IRegistry _registry;
		private readonly ReadOnlySpan<int> _entities;
		private int _current;

		public ViewEnumerator(IRegistry registry, ReadOnlySpan<int> entities)
		{
			_registry = registry;
			_entities = entities;
			_current = _entities.Length;
		}

		public bool MoveNext()
		{
			return --_current >= 0;
		}

		public void Reset()
		{
			_current = _entities.Length;
		}

		public Entity Current => new Entity(_registry, _entities[_current]);

		public void Dispose() { }
	}
}