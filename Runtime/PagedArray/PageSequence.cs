using System.Runtime.CompilerServices;
using Unity.IL2CPP.CompilerServices;

namespace Massive
{
	/// <summary>
	/// Reverse page sequence.
	/// </summary>
	[Il2CppSetOption(Option.NullChecks, false)]
	[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
	[Il2CppSetOption(Option.DivideByZeroChecks, false)]
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
			private readonly int _length;
			private int _page;
			private int _nextPageLength;
			private int _pageLength;

			public Enumerator(int pageSize, int length)
			{
				_pageSize = pageSize;
				_length = length;

				_page = _length == 0 ? 0 : (_length - 1) / _pageSize + 1;
				_pageLength = _nextPageLength = _length == 0 ? 0 : MathUtils.FastMod(_length - 1, _pageSize) + 1;
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

			public void Reset()
			{
				_page = _length == 0 ? 0 : (_length - 1) / _pageSize + 1;
				_pageLength = _nextPageLength = _length == 0 ? 0 : MathUtils.FastMod(_length - 1, _pageSize) + 1;
			}

			public (int PageIndex, int PageLength, int IndexOffset) Current
			{
				[MethodImpl(MethodImplOptions.AggressiveInlining)]
				get => (_page, _pageLength, _page * _pageSize);
			}
		}
	}
}
