using System;
using System.Runtime.CompilerServices;
using Unity.IL2CPP.CompilerServices;

namespace Massive
{
	[Il2CppSetOption(Option.NullChecks, false)]
	[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
	public class PagedArray<T> : IPagedArray
	{
		public T[][] Pages { get; private set; } = Array.Empty<T[]>();
		public int PagesCapacity { get; private set; }

		public int PageSize { get; }
		public int PageSizePower { get; }

		public PagedArray(int pageSize = Constants.DefaultPageSize)
		{
			InvalidPageSizeException.ThrowIfNotPowerOf2<T>(pageSize);

			PageSize = pageSize;
			PageSizePower = MathUtils.FastLog2(pageSize);
		}

		public Type ElementType => typeof(T);

		public ref T this[int index]
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => ref Pages[PageIndex(index)][IndexInPage(index)];
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Swap(int first, int second)
		{
			var firstPage = Pages[PageIndex(first)];
			var secondPage = Pages[PageIndex(second)];

			var firstIndex = IndexInPage(first);
			var secondIndex = IndexInPage(second);

			(firstPage[firstIndex], secondPage[secondIndex]) = (secondPage[secondIndex], firstPage[firstIndex]);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void EnsurePageAt(int index)
		{
			EnsurePage(PageIndex(index));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void EnsurePage(int page)
		{
			if (page >= PagesCapacity)
			{
				Pages = Pages.Resize(page + 1);
				PagesCapacity = page + 1;
			}

			Pages[page] ??= new T[PageSize];
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool HasPageAt(int index)
		{
			return HasPage(PageIndex(index));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool HasPage(int page)
		{
			return page < PagesCapacity && Pages[page] != null;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public PagedSpan<T> AsSpan(int length)
		{
			return new PagedSpan<T>(this, length);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Array GetPage(int page)
		{
			return Pages[page];
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private int PageIndex(int index)
		{
			return MathUtils.FastPowDiv(index, PageSizePower);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private int IndexInPage(int index)
		{
			return MathUtils.FastMod(index, PageSize);
		}
	}
}
