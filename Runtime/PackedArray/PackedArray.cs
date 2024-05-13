using System;
using System.Collections;
using System.Runtime.CompilerServices;
using Unity.IL2CPP.CompilerServices;

namespace Massive
{
	[Il2CppSetOption(Option.NullChecks, false)]
	[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
	public class PackedArray<T>
	{
		private readonly int _pageSize;
		private T[][] _pagedData;

		public PackedArray(int pageSize = Constants.PageSize)
		{
			if (!MathHelpers.IsPowerOfTwo(pageSize))
			{
				throw new Exception("Page capacity must be power of two!");
			}

			_pageSize = pageSize;
			_pagedData = new T[Constants.PagesAmount][];
		}

		public T[][] PagedData => _pagedData;

		public int PageSize => _pageSize;

		public int Capacity => _pagedData.Length * _pageSize;

		public ref T this[int index]
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => ref GetUnsafe(index);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public ref T GetUnsafe(int index)
		{
			int page = index / PageSize;
			return ref _pagedData[page][MathHelpers.FastMod(index, PageSize)];
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public ref T GetSafe(int index)
		{
			int page = index / PageSize;

			EnsurePage(page);

			return ref _pagedData[page][MathHelpers.FastMod(index, PageSize)];
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void EnsurePageForIndex(int index)
		{
			int page = index / PageSize;

			EnsurePage(page);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void EnsurePage(int page)
		{
			if (page >= _pagedData.Length)
			{
				Array.Resize(ref _pagedData, MathHelpers.GetNextPowerOf2(page + 1));
			}

			_pagedData[page] ??= new T[PageSize];
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool HasPageForIndex(int index)
		{
			int page = index / PageSize;
			return HasPage(page);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool HasPage(int page)
		{
			return page < _pagedData.Length && _pagedData[page] != null;
		}

		public void CopyTo(PackedArray<T> other, int length = int.MaxValue, Action<T[], T[], int> copyMethod = null)
		{
			copyMethod ??= Array.Copy;
			
			if (PageSize != other.PageSize)
			{
				throw new Exception("Can't copy packed arrays with different page size.");
			}

			int fullPages = Math.Min(length / _pageSize, _pagedData.Length);
			for (int pageIndex = 0; pageIndex < fullPages; pageIndex++)
			{
				var page = _pagedData[pageIndex];

				if (page != null)
				{
					other.EnsurePage(pageIndex);
					copyMethod(page, other.PagedData[pageIndex], _pageSize);
				}
			}

			if (fullPages < _pagedData.Length && _pagedData[fullPages] != null)
			{
				other.EnsurePage(fullPages);
				copyMethod(_pagedData[fullPages], other.PagedData[fullPages], MathHelpers.FastMod(length, _pageSize));
			}
		}

		public void ForEachPage(Action<T[], int> pageAction, int length = int.MaxValue)
		{
			int fullPages = Math.Min(length / _pageSize, _pagedData.Length);
			for (int pageIndex = 0; pageIndex < fullPages; pageIndex++)
			{
				var page = _pagedData[pageIndex];

				if (page != null)
				{
					pageAction(page, _pageSize);
				}
			}

			if (fullPages < _pagedData.Length && _pagedData[fullPages] != null)
			{
				pageAction(_pagedData[fullPages], MathHelpers.FastMod(length, _pageSize));
			}
		}
	}
}
