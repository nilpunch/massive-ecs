using System.Runtime.CompilerServices;
using Unity.IL2CPP.CompilerServices;

namespace Massive
{
	/// <summary>
	/// Reverse page sequence.
	/// </summary>
	[Il2CppSetOption(Option.NullChecks | Option.ArrayBoundsChecks, false)]
	public readonly struct GroupPageSequence
	{
		private readonly int _pageSize;
		private readonly Group _group;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public GroupPageSequence(int pageSize, Group group)
		{
			_pageSize = pageSize;
			_group = group;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Enumerator GetEnumerator()
		{
			return new Enumerator(_pageSize, _group);
		}

		public struct Enumerator
		{
			private readonly int _pageSize;
			private readonly Group _group;
			private int _page;
			private int _nextPageLength;
			private int _pageLength;

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public Enumerator(int pageSize, Group group)
			{
				_pageSize = pageSize;
				_group = group;

				var length = group.Count;
				_page = length == 0 ? 0 : (length - 1) / _pageSize + 1;
				_pageLength = _nextPageLength = length == 0 ? 0 : MathHelpers.FastMod(length - 1, _pageSize) + 1;
			}

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public bool MoveNext()
			{
				if (--_page >= 0)
				{
					_pageLength = _nextPageLength;
					_nextPageLength = _pageSize;

					return true;
				}

				return false;
			}

			public GroupPage Current
			{
				[MethodImpl(MethodImplOptions.AggressiveInlining)]
				get => new GroupPage(_page, _pageLength, _page * _pageSize, _group);
			}
		}
	}
}
