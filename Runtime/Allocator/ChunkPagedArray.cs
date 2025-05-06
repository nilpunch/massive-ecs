using System;
using System.Runtime.CompilerServices;
using Unity.IL2CPP.CompilerServices;

namespace Massive
{
	[Il2CppSetOption(Option.NullChecks, false)]
	[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
	public class ChunkPagedArray : IPagedArray
	{
		public Chunk[][] Pages { get; private set; } = Array.Empty<Chunk[]>();
		public int PagesCapacity { get; private set; }

		public int PageSize { get; }
		public int PageSizePower { get; }

		public ChunkPagedArray(int pageSize = Constants.DefaultPageSize)
		{
			if (!MathUtils.IsPowerOfTwo(pageSize))
			{
				throw new Exception($"{MassiveAssert.Library} Page size must be power of two! Type:{typeof(ChunkPagedArray).GetGenericName()}.");
			}

			PageSize = pageSize;
			PageSizePower = MathUtils.FastLog2(pageSize);
		}

		public ref Chunk this[int index]
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => ref Pages[PageIndex(index)][IndexInPage(index)];
		}

		public Type ElementType => typeof(Chunk);

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

			if (Pages[page] is null)
			{
				var validChunk = default(Chunk);
				validChunk.Version = 1U;
				Pages[page] = new Chunk[PageSize];
				Array.Fill(Pages[page], validChunk);
			}
			Pages[page] ??= new Chunk[PageSize];
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
