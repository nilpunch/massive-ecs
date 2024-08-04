using System;
using System.Runtime.CompilerServices;
using Unity.IL2CPP.CompilerServices;

namespace Massive
{
	[Il2CppSetOption(Option.NullChecks, false)]
	[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
	[Il2CppSetOption(Option.DivideByZeroChecks, false)]
	public class PagedArray<T>
	{
		private const int DefaultPagesAmount = 4;

		private readonly int _pageSizePower;
		private T[][] _pages;

		public PagedArray(int pageSize = Constants.DefaultPageSize)
		{
			if (!MathHelpers.IsPowerOfTwo(pageSize))
			{
				throw new Exception("Page capacity must be power of two!");
			}

			PageSize = pageSize;
			_pageSizePower = MathHelpers.FastLog2(pageSize);
			_pages = new T[DefaultPagesAmount][];
		}

		public T[][] Pages
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => _pages;
		}

		public int PageSize { get; }

		public ref T this[int index]
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => ref _pages[PageIndex(index)][IndexInPage(index)];
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Swap(int first, int second)
		{
			var firstPage = _pages[PageIndex(first)];
			var secondPage = _pages[PageIndex(second)];

			var firstIndex = IndexInPage(first);
			var secondIndex = IndexInPage(second);

			(firstPage[firstIndex], secondPage[secondIndex]) = (secondPage[secondIndex], firstPage[firstIndex]);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void EnsurePageForIndex(int index)
		{
			EnsurePage(PageIndex(index));
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
			return HasPage(PageIndex(index));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool HasPage(int page)
		{
			return page < _pages.Length && _pages[page] != null;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public PagedSpan<T> AsSpan(int length)
		{
			return new PagedSpan<T>(this, length);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private int PageIndex(int index)
		{
			return MathHelpers.FastPowDiv(index, _pageSizePower);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private int IndexInPage(int index)
		{
			return MathHelpers.FastMod(index, PageSize);
		}
	}
}
