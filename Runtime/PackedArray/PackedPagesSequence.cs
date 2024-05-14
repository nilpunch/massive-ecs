using System;
using System.Runtime.CompilerServices;
using Unity.IL2CPP.CompilerServices;

namespace Massive
{
	[Il2CppSetOption(Option.NullChecks, false)]
	[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
	public readonly struct PackedPagesSequence<T>
	{
		public readonly PackedArray<T> PackedArray;
		public readonly int Length;

		public PackedPagesSequence(PackedArray<T> packedArray, int length)
		{
			PackedArray = packedArray;
			Length = length;
		}
		
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Enumerator GetEnumerator()
		{
			return new Enumerator(PackedArray, Length);
		}

		public ref struct Enumerator
		{
			private readonly T[][] _pagedData;
			private readonly int _pagesAmount;
			private readonly int _pageSize;
			private readonly int _length;
			private int _page;
			private int _pageLength;

			public Enumerator(PackedArray<T> packedArray, int length)
			{
				_pagedData = packedArray.PagedData;
				_pageSize = packedArray.PageSize;
				_length = length;

				if (length > packedArray.Capacity)
				{
					throw new ArgumentOutOfRangeException(nameof(length));
				}

				_pagesAmount = packedArray.PagesAmount;
				_page = _length / _pageSize + 1;
				_pageLength = 0;
			}

			public bool MoveNext()
			{
				if (--_page >= 0)
				{
					bool isLastPage = _page == _pagesAmount - 1;
					_pageLength = isLastPage
						? MathHelpers.FastMod(_length, _pageSize)
						: _pageSize;
					return true;
				}

				return false;
			}

			public void Reset()
			{
				_page = _length / _pageSize + 1;
				_pageLength = 0;
			}

			public (T[] Page, int Count) Current => (_pagedData[_page], _pageLength);
		}
	}
}
