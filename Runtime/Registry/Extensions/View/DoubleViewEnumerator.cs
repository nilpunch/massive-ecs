using System;

namespace Massive.ECS
{
	public ref struct DoubleViewEnumerator
	{
		private readonly IRegistry _registry;
		private ISet _min;
		private ISet _max;
		private ReadOnlySpan<int> _ids;
		private int _current;

		public DoubleViewEnumerator(IRegistry registry, ISet set1, ISet set2)
		{
			_registry = registry;

			if (set1.AliveCount > set2.AliveCount)
			{
				_min = set2;
				_max = set1;
			}
			else
			{
				_min = set1;
				_max = set2;
			}
			
			_current = _min.AliveCount;
			_ids = _min.AliveIds;
		}

		public bool MoveNext()
		{
			do
			{
				--_current;
			} while (_current >= 0 && !_max.IsAlive(_ids[_current]));
			
			return _current >= 0;
		}

		public void Reset()
		{
			if (_min.AliveCount > _max.AliveCount)
			{
				(_min, _max) = (_max, _min);
			}
			
			_current = _min.AliveCount;
			_ids = _min.AliveIds;
		}

		public Entity Current => new Entity(_registry, _ids[_current]);

		public void Dispose() { }
	}
}