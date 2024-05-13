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
			private int _page;
			private Span<T> _currentPage;
			private int _index;

			public Enumerator(PackedArray<T> packedArray, int length)
			{
				_pagedData = packedArray.PagedData;
				_pageSize = packedArray.PageSize;
				_length = length;

				_page = _length / _pageSize - 1;
				_index = _length % _pageSize + 1;
				_currentPage = new Span<T>(_pagedData[_page]);
			}

			public bool MoveNext()
			{
				if (--_index >= 0)
				{
					return true;
				}

				while (--_page >= 0)
				{
				}

				if (_page >= 0)
				{
					_index = _pageSize - 1;
					_currentPage = new Span<T>(_pagedData[_page]);
					return true;
				}
				
				return false;
			}

			public void Reset()
			{
				_page = _length / _pageSize - 1;
				_index = _length % _pageSize + 1;
				_currentPage = new Span<T>(_pagedData[_page]);
			}

			public ref T Current => ref _currentPage[_index];
		}
	}
}
