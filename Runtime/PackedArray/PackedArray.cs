using System;
using System.Runtime.CompilerServices;
using Unity.IL2CPP.CompilerServices;

namespace Massive
{
	[Il2CppSetOption(Option.NullChecks, false)]
	[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
	public class PagedArray<T>
	{
		private T[][] _pages;

		public PagedArray(int pageSize = Constants.PageSize)
		{
			if (!MathHelpers.IsPowerOfTwo(pageSize))
			{
				throw new Exception("Page capacity must be power of two!");
			}

			PageSize = pageSize;
			_pages = new T[Constants.PagesAmount][];
		}

		public T[][] Pages => _pages;

		public int PageSize { get; }

		public ref T this[int index]
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				int page = index / PageSize;
				return ref _pages[page][MathHelpers.FastMod(index, PageSize)];
			}
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
			if (page >= _pages.Length)
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
			return page < _pages.Length && _pages[page] != null;
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

			foreach (var (pageIndex, pageLength, _) in new PageSequence(PageSize, length))
			{
				other.EnsurePage(pageIndex);
				copyMethod(Pages[pageIndex], other.Pages[pageIndex], pageLength);
			}
		}
	}
}
