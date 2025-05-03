using System;
using System.Runtime.CompilerServices;
using Unity.IL2CPP.CompilerServices;

namespace Massive
{
	/// <summary>
	/// Reverse iterator for <see cref="PagedArray{T}"/>.
	/// </summary>
	[Il2CppSetOption(Option.NullChecks, false)]
	[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
	public readonly struct PagedSpan<T>
	{
		private readonly PagedArray<T> _pagedArray;
		private readonly int _length;

		public PagedSpan(PagedArray<T> pagedArray, int length)
		{
			_pagedArray = pagedArray;
			_length = length;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Enumerator GetEnumerator()
		{
			return new Enumerator(_pagedArray, _length);
		}

		public ref struct Enumerator
		{
			private readonly T[][] _pagedData;
			private readonly int _pageSize;
			private int _pageIndex;
			private Span<T> _currentPage;
			private int _indexInPage;

			public Enumerator(PagedArray<T> pagedArray, int length)
			{
				_pagedData = pagedArray.Pages;
				_pageSize = pagedArray.PageSize;

				_pageIndex = length / _pageSize;
				_indexInPage = MathUtils.FastMod(length, _pageSize);
				_currentPage = _pageIndex < _pagedData.Length ? new Span<T>(_pagedData[_pageIndex]) : Span<T>.Empty;
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

			public ref T Current
			{
				[MethodImpl(MethodImplOptions.AggressiveInlining)]
				get => ref _currentPage[_indexInPage];
			}
		}
	}
}
