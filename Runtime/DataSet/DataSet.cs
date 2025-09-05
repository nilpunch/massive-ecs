#if !MASSIVE_DISABLE_ASSERT
#define MASSIVE_ASSERT
#endif

using System;
using System.Runtime.CompilerServices;
using Unity.IL2CPP.CompilerServices;

namespace Massive
{
	/// <summary>
	/// Data extension for <see cref="BitSet"/>.
	/// Does not reset the data for added elements.
	/// Does not preserve data when elements are moved.
	/// </summary>
	[Il2CppSetOption(Option.NullChecks, false)]
	[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
	public class DataSet<T> : BitSet, IDataSet
	{
		public const int EndFreePage = Constants.InvalidId;

		public struct Page
		{
			public int DataIndex;
			public int NextFreePage;
		}

		public T[][] Data { get; private set; } = Array.Empty<T[]>();

		public Page[] Pages { get; protected internal set; } = Array.Empty<Page>();

		public int UsedPages { get; protected internal set; }

		public int NextFreePage { get; protected internal set; } = EndFreePage;

		public int PageSize { get; }

		public Type ElementType { get; }

		public int PageSizePower { get; }

		public int PageSizeMinusOne { get; }

		public DataSet(int pageSize = Constants.DefaultPageSize)
		{
			InvalidPageSizeException.ThrowIfNotPowerOf2<T>(pageSize);

			PageSize = pageSize;
			PageSizePower = MathUtils.FastLog2(pageSize);
			PageSizeMinusOne = pageSize - 1;
		}

		/// <summary>
		/// Gets a reference to the data associated with the specified ID.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public ref T Get(int id)
		{
			var index = Pages[id >> 6].DataIndex + (id & 63);
			return ref Data[index >> PageSizePower][index & PageSizeMinusOne];
		}

		/// <summary>
		/// Adds the specified ID if not present and sets the associated data.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Set(int id, T data)
		{
			NegativeArgumentException.ThrowIfNegative(id);

			var id0 = id >> 6;
			var id1 = id >> 12;

			if (id1 >= Bits1.Length)
			{
				Bits1 = Bits1.Resize(MathUtils.NextPowerOf2(id1 + 1));
				Bits0 = Bits0.Resize(Bits1.Length << 6);
			}

			var offsetInPage = id & 63;
			var bit0 = 1UL << offsetInPage;
			var bit1 = 1UL << (id0 & 63);

			if (Bits0[id0] == 0UL)
			{
				Bits1[id1] |= bit1;
				AddPage(id0);
			}
			Bits0[id0] |= bit0;

			for (var i = 0; i < RemoveOnAddCount; i++)
			{
				RemoveOnAdd[i].Remove(id);
			}

			var index = Pages[id0].DataIndex + offsetInPage;
			Data[index >> PageSizePower][index & PageSizeMinusOne] = data;
			NotifyAfterAdded(id);
			Components?.Set(id, ComponentId);
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

		protected override void RemoveAllPages()
		{
			NextFreePage = EndFreePage;
			UsedPages = 0;
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

		/// <summary>
		/// Copies the data from one index to another.
		/// </summary>
		public override void CopyData(int sourceId, int destinationId)
		{
			Data[destinationId] = Data[sourceId];
		}

		Array IDataSet.GetPage(int page)
		{
			return Data[page];
		}

		object IDataSet.GetRaw(int id) => Get(id);

		void IDataSet.SetRaw(int id, object value) => Set(id, (T)value);

		/// <summary>
		/// Creates and returns a new data set that is an exact copy of this one.
		/// All data is copied by value.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public DataSet<T> Clone()
		{
			var clone = new DataSet<T>(PageSize);
			CopyTo(clone);
			return clone;
		}

		/// <summary>
		/// Copies all data and sparse state from this set into the specified one.
		/// All data is copied by value.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void CopyTo(DataSet<T> other)
		{
			// IncompatiblePageSizeException.ThrowIfIncompatible(Data, other.Data);

			CopyBitsTo(other);

			foreach (var page in new PageSequence(PageSize, UsedPages << 6))
			{
				other.EnsurePage(page.Index);

				var sourcePage = Data[page.Index];
				var destinationPage = other.Data[page.Index];

				Array.Copy(sourcePage, destinationPage, page.Length);
			}

			if (UsedPages > other.Pages.Length)
			{
				other.Pages = other.Pages.ResizeToNextPowOf2(UsedPages);
			}

			Array.Copy(Pages, other.Pages, UsedPages);

			other.UsedPages = UsedPages;
			other.NextFreePage = NextFreePage;
		}
	}
}
