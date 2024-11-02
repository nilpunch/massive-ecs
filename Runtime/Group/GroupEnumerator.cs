using System.Runtime.CompilerServices;

namespace Massive
{
	public ref struct GroupEnumerator
	{
		private readonly SparseSet _mainSet;
		private readonly Group _group;
		private int _index;

		public GroupEnumerator(Group group)
		{
			_group = group;
			_mainSet = group.MainSet;
			_index = group.Count;
		}

		public int Current
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => _mainSet.Ids[_index];
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool MoveNext()
		{
			if (--_index > _group.Count)
			{
				_index = _group.Count - 1;
			}

			return _index >= 0;
		}
	}
}
