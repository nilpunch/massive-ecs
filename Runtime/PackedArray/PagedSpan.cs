using System;
using System.Runtime.CompilerServices;
using Unity.IL2CPP.CompilerServices;

namespace Massive
{
	[Il2CppSetOption(Option.NullChecks, false)]
	[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
	public readonly struct PagedSpan<T>
	{
		public readonly PagedArray<T> PagedArray;
		public readonly int Length;

		public PagedSpan(PagedArray<T> pagedArray, int length)
		{
			PagedArray = pagedArray;
			Length = length;
		}

		public ref T this[int index]
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => ref PagedArray.GetUnsafe(index);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Enumerator GetEnumerator()
		{
			return new Enumerator(PagedArray, Length);
		}

		public ref struct Enumerator
		{
			private readonly T[][] _pagedData;
			private readonly int _pageSize;
			private readonly int _length;
			private int _pageIndex;
			private Span<T> _currentPage;
			private int _indexInPage;

			public Enumerator(PagedArray<T> pagedArray, int length)
			{
				_pagedData = pagedArray.Pages;
				_pageSize = pagedArray.PageSize;
				_length = length;

				_pageIndex = _length / _pageSize;
				_indexInPage = MathHelpers.FastMod(_length, _pageSize);
				_currentPage = new Span<T>(_pagedData[_pageIndex]);
			}

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public bool MoveNext()
			{
				if (--_indexInPage >= 0)
				{
					return true;
				}

				if (--_pageIndex >= 0)
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
