using System;
using System.Runtime.CompilerServices;
using Unity.IL2CPP.CompilerServices;

namespace Massive
{
	[Il2CppSetOption(Option.NullChecks, false)]
	[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
	public readonly struct PackedSpan<T>
	{
		public readonly PackedArray<T> PackedArray;
		public readonly int Length;

		public PackedSpan(PackedArray<T> packedArray, int length)
		{
			PackedArray = packedArray;
			Length = length;
		}

		public ref T this[int index]
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => ref PackedArray.GetUnsafe(index);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Enumerator GetEnumerator()
		{
			return new Enumerator(PackedArray, Length);
		}

		public ref struct Enumerator
		{
			private readonly T[][] _pagedData;
			private readonly int _pageSize;
			private readonly int _length;
			private int _pageIndex;
			private Span<T> _currentPage;
			private int _indexInPage;

			public Enumerator(PackedArray<T> packedArray, int length)
			{
				_pagedData = packedArray.PagedData;
				_pageSize = packedArray.PageSize;
				_length = length;

				_pageIndex = Math.Min(_length / _pageSize, _pagedData.Length);
				_indexInPage = MathHelpers.FastMod(_length, _pageSize);
				_currentPage = new Span<T>(_pagedData[_pageIndex]);
			}

			public bool MoveNext()
			{
				if (--_indexInPage >= 0)
				{
					return true;
				}

				while (--_pageIndex >= 0)
				{
				}

				if (_pageIndex >= 0)
				{
					_indexInPage = _pageSize - 1;
					_currentPage = new Span<T>(_pagedData[_pageIndex]);
					return true;
				}

				return false;
			}

			public void Reset()
			{
				_pageIndex = Math.Min(_length / _pageSize, _pagedData.Length);
				_indexInPage = MathHelpers.FastMod(_length, _pageSize);
				_currentPage = new Span<T>(_pagedData[_pageIndex]);
			}

			public ref T Current => ref _currentPage[_indexInPage];
		}
	}
}
