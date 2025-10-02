using System.Runtime.CompilerServices;
using Unity.IL2CPP.CompilerServices;

namespace Massive
{
	/// <summary>
	/// Reverse page sequence.
	/// </summary>
	[Il2CppSetOption(Option.NullChecks, false)]
	[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
	public readonly struct PageSequence
	{
		private readonly int _pageSize;
		private readonly int _length;

		public PageSequence(int pageSize, int length)
		{
			_pageSize = pageSize;
			_length = length;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Enumerator GetEnumerator()
		{
			return new Enumerator(_pageSize, _length);
		}

		public struct Enumerator
		{
			private readonly int _pageSize;
			private int _page;
			private int _nextPageLength;
			private int _pageLength;

			public Enumerator(int pageSize, int length)
			{
				_pageSize = pageSize;
				_page = length == 0 ? 0 : (length - 1) / _pageSize + 1;
				_pageLength = _nextPageLength = length == 0 ? 0 : MathUtils.FastMod(length - 1, _pageSize) + 1;
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

			public Page Current
			{
				[MethodImpl(MethodImplOptions.AggressiveInlining)]
				get => new Page(_page, _pageLength, _page * _pageSize);
			}
		}

		public readonly struct Page
		{
			public readonly int Index;

			public readonly int Length;

			public readonly int Offset;

			public Page(int index, int length, int offset)
			{
				Index = index;
				Length = length;
				Offset = offset;
			}
		}
	}
}
