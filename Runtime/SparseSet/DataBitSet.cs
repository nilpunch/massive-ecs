using System;
using System.Runtime.CompilerServices;
using Unity.IL2CPP.CompilerServices;

namespace Massive
{
	[Il2CppSetOption(Option.NullChecks, false)]
	[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
	public class DataBitSet<T> : TagSet
	{
		public const int EndFreePage = Constants.InvalidId;

		public struct Page
		{
			public int DataIndex;
			public int NextFreePage;

			public Page(int dataIndex, int nextFreePage)
			{
				DataIndex = dataIndex;
				NextFreePage = nextFreePage;
			}
		}

		public T[][] Data { get; private set; } = Array.Empty<T[]>();

		public Page[] Pages { get; private set; } = Array.Empty<Page>();

		public int UsedPages { get; private set; }

		public int NextFreePage { get; private set; } = EndFreePage;

		public int PageSize { get; }

		public int PageSizePower { get; }

		public int PageSizeMinusOne { get; }

		public DataBitSet(int pageSize = Constants.DefaultPageSize)
		{
			InvalidPageSizeException.ThrowIfNotPowerOf2<T>(pageSize);

			PageSize = pageSize;
			PageSizePower = MathUtils.FastLog2(pageSize);
			PageSizeMinusOne = pageSize - 1;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public ref T Get(int id)
		{
			var pageOffset = Pages[id >> 6].DataIndex;

			return ref Data[pageOffset >> PageSizePower][(pageOffset & PageSizeMinusOne) + (id & 63)];
		}

		protected override void AddPage(int page)
		{
			if (page >= Pages.Length)
			{
				Pages = Pages.Resize(MathUtils.NextPowerOf2(page + 1));
			}

			if (NextFreePage != EndFreePage)
			{
				var nextFreePage = NextFreePage;
				NextFreePage = Pages[nextFreePage].NextFreePage;
				Pages[page].DataIndex = Pages[nextFreePage].DataIndex;
			}
			else
			{
				var nextPageDataIndex = UsedPages << 6;
				EnsurePageAt(nextPageDataIndex);
				UsedPages++;
				Pages[page].DataIndex = nextPageDataIndex;
			}
		}

		protected override void RemovePage(int page)
		{
			Pages[page].NextFreePage = NextFreePage;
			NextFreePage = page;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void EnsurePageAt(int id)
		{
			EnsurePage(id >> PageSizePower);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void EnsurePage(int page)
		{
			if (page >= Data.Length)
			{
				Data = Data.Resize(page + 1);
			}

			Data[page] ??= new T[PageSize];
		}
	}
}
