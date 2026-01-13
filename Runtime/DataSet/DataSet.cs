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
	/// Resets data to default value for added elements.
	/// </summary>
	[Il2CppSetOption(Option.NullChecks, false)]
	[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
	public class DataSet<T> : BitSet, IDataSet
	{
		public T DefaultValue { get; }

		public T[][] PagedData { get; private set; } = Array.Empty<T[]>();

		private T[][] DataPagePool { get; set; } = Array.Empty<T[]>();

		private int PoolCount { get; set; }

		public DataSet(T defaultValue = default)
		{
			DefaultValue = defaultValue;
		}

		/// <summary>
		/// Gets a reference to the data associated with the specified ID.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public ref T Get(int id)
		{
			InvalidGetOperationException.ThrowIfNotAdded(this, id);
			return ref PagedData[id >> Constants.PageSizePower][id & Constants.PageSizeMinusOne];
		}

		/// <summary>
		/// Adds the specified ID if not present and sets the associated data.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Set(int id, T data)
		{
			NegativeArgumentException.ThrowIfNegative(id);

			var bitsIndex = id >> 6;
			var blockIndex = id >> 12;

			EnsureBlocksCapacityAt(blockIndex);

			var bitsMask = 1UL << (id & 63);
			var blockMask = 1UL << (bitsIndex & 63);
			var pageIndex = id >> Constants.PageSizePower;

			if ((Bits[bitsIndex] & bitsMask) != 0UL)
			{
				PagedData[pageIndex][id & Constants.PageSizeMinusOne] = data;
				return;
			}

			if (Bits[bitsIndex] == 0UL)
			{
				EnsurePageInternal(pageIndex);
				NonEmptyBlocks[blockIndex] |= blockMask;
			}
			Bits[bitsIndex] |= bitsMask;
			if (Bits[bitsIndex] == ulong.MaxValue)
			{
				SaturatedBlocks[blockIndex] |= blockMask;
			}

			PagedData[pageIndex][id & Constants.PageSizeMinusOne] = data;

			NotifyAfterAdded(id);

			for (var i = 0; i < RemoveOnAddCount; i++)
			{
				RemoveOnAdd[i].RemoveBit(bitsIndex, bitsMask);
			}
		}

		public override void EnsurePage(int page)
		{
			EnsurePageInternal(page);
		}

		public override void CopyData(int sourceId, int destinationId)
		{
			Get(destinationId) = Get(sourceId);
		}

		protected override void ClearData(int id)
		{
			PagedData[id >> Constants.PageSizePower][id & Constants.PageSizeMinusOne] = DefaultValue;
		}

		protected override void FreePage(int page)
		{
			FreePageInternal(page);
		}

		protected override void FreeAllPages()
		{
			for (var i = 0; i < PagedData.Length; i++)
			{
				if (PagedData[i] != null)
				{
					FreePageInternal(i);
				}
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		protected internal void EnsurePageInternal(int page)
		{
			if (page >= PagedData.Length)
			{
				PagedData = PagedData.Resize(page + 1);
			}

			PagedData[page] ??= CreatePage();
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void FreePageInternal(int page)
		{
			if (PoolCount >= DataPagePool.Length)
			{
				DataPagePool = DataPagePool.ResizeToNextPowOf2(PoolCount + 1);
			}

			DataPagePool[PoolCount++] = PagedData[page];
			PagedData[page] = null;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private T[] CreatePage()
		{
			if (PoolCount > 0)
			{
				return DataPagePool[--PoolCount];
			}
			else
			{
				var newPage = new T[Constants.PageSize];
				Array.Fill(newPage, DefaultValue);
				return newPage;
			}
		}

		/// <summary>
		/// Creates and returns a new data set that is an exact copy of this one.
		/// All data is copied by value.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public DataSet<T> Clone()
		{
			var clone = new DataSet<T>(DefaultValue);
			CopyTo(clone);
			return clone;
		}

		/// <summary>
		/// Copies all data and bitset state from this set into the specified one.
		/// All data is copied by value.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void CopyTo(DataSet<T> other)
		{
			CopyBitSetTo(other);

			var blocksLength = BlocksCapacity;
			var pageMasksNegative = Constants.PageMasksNegative;
			var deBruijn = MathUtils.DeBruijn;
			for (var blockIndex = 0; blockIndex < blocksLength; blockIndex++)
			{
				var block = NonEmptyBlocks[blockIndex];
				var pageOffset = blockIndex << Constants.PagesInBlockPower;
				while (block != 0UL)
				{
					var blockBit = (int)deBruijn[(int)(((block & (ulong)-(long)block) * 0x37E84A99DAE458FUL) >> 58)];
					var pageIndexMod = blockBit >> Constants.PageMaskShift;

					var pageIndex = pageOffset + pageIndexMod;
					other.EnsurePageInternal(pageIndex);

					var page = PagedData[pageIndex];
					var otherPage = other.PagedData[pageIndex];

#if UNITY_EDITOR || NET
					Array.Copy(page, otherPage, Constants.PageSize);
#else
					for (var i = 0; i < Constants.PageSize; i++)
					{
						otherPage[i] = page[i];
					}
#endif

					block &= pageMasksNegative[pageIndexMod];
				}
			}
		}

		BitSet IDataSet.BitSet => this;

		Type IDataSet.ElementType => typeof(T);

		Type IDataSet.ArrayType => typeof(T[]);

		Array IDataSet.GetPage(int page) => PagedData[page];

		object IDataSet.GetRaw(int id) => Get(id);

		void IDataSet.SetRaw(int id, object value) => Set(id, (T)value);

		DataPageEnumerable IDataSet.GetDataPages() => new DataPageEnumerable(this);
	}
}
