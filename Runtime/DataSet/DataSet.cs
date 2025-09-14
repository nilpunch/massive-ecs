#if !MASSIVE_DISABLE_ASSERT
#define MASSIVE_ASSERT
#endif

using System;
using System.Runtime.CompilerServices;
using Unity.IL2CPP.CompilerServices;

namespace Massive
{
	/// <summary>
	/// Data extension for <see cref="SparseSet"/>.
	/// Does not reset the data for added elements.
	/// Does not preserve data when elements are moved.
	/// </summary>
	[Il2CppSetOption(Option.NullChecks, false)]
	[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
	public class DataSet<T> : SparseSet, IDataSet
	{
		public T[][] PagedData { get; private set; } = Array.Empty<T[]>();

		private T[][] DataPagePool { get; set; } = Array.Empty<T[]>();

		private int PoolCount { get; set; }

		public DataSet(int pageSize = Constants.DefaultPageSize) : base(pageSize)
		{
			InvalidPageSizeException.ThrowIfNotPowerOf2<T>(pageSize);
			InvalidPageSizeException.ThrowIfTooLargeOrTooSmall<T>(pageSize);
		}

		/// <summary>
		/// Gets a reference to the data associated with the specified ID.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public ref T Get(int id)
		{
			return ref PagedData[id >> PageSizePower][id & PageSizeMinusOne];
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
				Bits1 = Bits1.ResizeToNextPowOf2(id1 + 1);
				Bits0 = Bits0.Resize(Bits1.Length << 6);
			}

			var mod64 = id & 63;
			var bit0 = 1UL << mod64;
			var bit1 = 1UL << (id0 & 63);

			var pageIndex = id >> PageSizePower;
			var pageMask1 = PageMask1 << ((pageIndex & PagesInBits1MinusOne) << MaskShiftPower);
			if ((Bits1[id1] & pageMask1) == 0UL)
			{
				EnsurePage(pageIndex);
			}

			if (Bits0[id0] == 0UL)
			{
				Bits1[id1] |= bit1;
			}
			Bits0[id0] |= bit0;

			PagedData[id >> PageSizePower][id & PageSizeMinusOne] = data;

			for (var i = 0; i < RemoveOnAddCount; i++)
			{
				RemoveOnAdd[i].Remove(id);
			}

			Components?.Set(id, ComponentId);
			NotifyAfterAdded(id);
		}

		protected override void AllocPage(int page)
		{
			EnsurePage(page);
		}

		protected override void FreePage(int page)
		{
			if (PoolCount >= DataPagePool.Length)
			{
				DataPagePool = DataPagePool.ResizeToNextPowOf2(PoolCount + 1);
			}

			DataPagePool[PoolCount++] = PagedData[page];
			PagedData[page] = null;
		}

		protected override void FreeAllPages()
		{
			for (var i = 0; i < PagedData.Length; i++)
			{
				if (PagedData[i] != null)
				{
					FreePage(i);
				}
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void EnsurePageAt(int id)
		{
			EnsurePage(id >> PageSizePower);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void EnsurePage(int page)
		{
			if (page >= PagedData.Length)
			{
				PagedData = PagedData.Resize(page + 1);
			}

			PagedData[page] ??= CreatePage();
		}

		private T[] CreatePage()
		{
			return PoolCount > 0 ? DataPagePool[--PoolCount] : new T[PageSize];
		}

		/// <summary>
		/// Copies the data from one index to another.
		/// </summary>
		public override void CopyData(int sourceId, int destinationId)
		{
			Get(destinationId) = Get(sourceId);
		}

		Array IDataSet.GetPage(int page)
		{
			return PagedData[page];
		}

		Type IDataSet.ElementType => typeof(T);

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
			IncompatiblePageSizeException.ThrowIfIncompatible(this, other);

			CopyBitsTo(other);

			for (var i = 0; i < PagedData.Length; i++)
			{
				if (PagedData[i] != null)
				{
					other.EnsurePage(i);

					var sourcePage = PagedData[i];
					var destinationPage = other.PagedData[i];

					Array.Copy(sourcePage, destinationPage, PageSize);
				}
			}
		}
	}
}
