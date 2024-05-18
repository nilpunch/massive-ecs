using System;
using System.Runtime.CompilerServices;
using Unity.IL2CPP.CompilerServices;

namespace Massive
{
	[Il2CppSetOption(Option.NullChecks, false)]
	[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
	public class PagedArray<T>
	{
		private readonly int _pageSize;
		private T[][] _pages;

		public PagedArray(int pageSize = Constants.PageSize)
		{
			if (!MathHelpers.IsPowerOfTwo(pageSize))
			{
				throw new Exception("Page capacity must be power of two!");
			}

			_pageSize = pageSize;
			_pages = new T[Constants.PagesAmount][];
		}

		public T[][] Pages => _pages;
		
		public int PagesAmount => _pages.Length;

		public int PageSize => _pageSize;

		public int Capacity => PagesAmount * _pageSize;

		public ref T this[int index]
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => ref GetUnsafe(index);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public ref T GetUnsafe(int index)
		{
			int page = index / PageSize;
			return ref _pages[page][MathHelpers.FastMod(index, PageSize)];
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public ref T GetSafe(int index)
		{
			int page = index / PageSize;

			EnsurePage(page);

			return ref _pages[page][MathHelpers.FastMod(index, PageSize)];
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Swap(int first, int second)
		{
			var pageFirst = _pages[first / PageSize];
			var pageSecond = _pages[second / PageSize];

			var firstIndex = MathHelpers.FastMod(first, PageSize);
			var secondIndex = MathHelpers.FastMod(second, PageSize);

			(pageFirst[firstIndex], pageSecond[secondIndex]) = (pageSecond[secondIndex], pageFirst[firstIndex]);
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
			if (page >= PagesAmount)
			{
				Array.Resize(ref _pages, MathHelpers.GetNextPowerOf2(page + 1));
			}

			_pages[page] ??= new T[PageSize];
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
			return page < PagesAmount && _pages[page] != null;
		}

		public PagedSpan<T> AsSpan(int length)
		{
			return new PagedSpan<T>(this, length);
		}

		public void CopyTo(PagedArray<T> other, int length = int.MaxValue, Action<T[], T[], int> copyMethod = null)
		{
			copyMethod ??= Array.Copy;
			
			if (PageSize != other.PageSize)
			{
				throw new Exception("Can't copy packed arrays with different page size.");
			}

			int fullPages = Math.Min(length / PageSize, PagesAmount);
			for (int pageIndex = 0; pageIndex < fullPages; pageIndex++)
			{
				var page = _pages[pageIndex];

				if (page != null)
				{
					other.EnsurePage(pageIndex);
					copyMethod(page, other.Pages[pageIndex], PageSize);
				}
			}

			if (fullPages < PagesAmount && _pages[fullPages] != null)
			{
				other.EnsurePage(fullPages);
				copyMethod(_pages[fullPages], other.Pages[fullPages], MathHelpers.FastMod(length, PageSize));
			}
		}

		public void ForEachPage(Action<T[], int> pageAction, int length = int.MaxValue)
		{
			int fullPages = Math.Min(length / PageSize, PagesAmount);
			for (int pageIndex = 0; pageIndex < fullPages; pageIndex++)
			{
				var page = _pages[pageIndex];

				if (page != null)
				{
					pageAction(page, PageSize);
				}
			}

			if (fullPages < PagesAmount && _pages[fullPages] != null)
			{
				pageAction(_pages[fullPages], MathHelpers.FastMod(length, PageSize));
			}
		}
	}
}
