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
		public struct Block
		{
			public int PageIndex;
			public int StartInPage;
			public int NextFreeBlock;
		}

		public T[][] PagedData { get; private set; } = Array.Empty<T[]>();

		public Block[] Blocks { get; protected internal set; } = Array.Empty<Block>();

		public int UsedBlocks { get; protected internal set; }

		public int NextFreeBlock { get; protected internal set; } = Constants.InvalidId;

		public int PageSize { get; }

		public int PageSizePower { get; }

		public int PageSizeMinusOne { get; }

		public DataSet(int pageSize = Constants.DefaultPageSize)
		{
			InvalidPageSizeException.ThrowIfNotPowerOf2<T>(pageSize);
			InvalidPageSizeException.ThrowIfTooSmall<T>(pageSize);

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
			var block = Blocks[id >> 6];
			return ref PagedData[block.PageIndex][block.StartInPage + (id & 63)];
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

			if (Bits0[id0] == 0UL)
			{
				Bits1[id1] |= bit1;
				AllocBlock(id0);
			}
			Bits0[id0] |= bit0;

			var block = Blocks[id0];
			PagedData[block.PageIndex][block.StartInPage + mod64] = data;

			for (var i = 0; i < RemoveOnAddCount; i++)
			{
				RemoveOnAdd[i].Remove(id);
			}

			Components?.Set(id, ComponentId);
			NotifyAfterAdded(id);
		}

		protected override void AllocBlock(int block)
		{
			if (block >= Blocks.Length)
			{
				Blocks = Blocks.ResizeToNextPowOf2(block + 1);
			}

			if (NextFreeBlock != Constants.InvalidId)
			{
				var nextFreePage = NextFreeBlock;
				NextFreeBlock = Blocks[nextFreePage].NextFreeBlock;
				Blocks[block] = Blocks[nextFreePage];
				return;
			}

			var startIndex = UsedBlocks << 6;
			var pageIndex = startIndex >> PageSizePower;
			var startInPage = startIndex & PageSizeMinusOne;
			EnsurePage(pageIndex);
			Blocks[block].PageIndex = pageIndex;
			Blocks[block].StartInPage = startInPage;
			UsedBlocks++;
		}

		protected override void FreeBlock(int block)
		{
			Blocks[block].NextFreeBlock = NextFreeBlock;
			NextFreeBlock = block;
		}

		protected override void FreeAllBlocks()
		{
			NextFreeBlock = Constants.InvalidId;
			UsedBlocks = 0;
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

			PagedData[page] ??= new T[PageSize];
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

			foreach (var page in new PageSequence(PageSize, UsedBlocks << 6))
			{
				other.EnsurePage(page.Index);

				var sourcePage = PagedData[page.Index];
				var destinationPage = other.PagedData[page.Index];

				Array.Copy(sourcePage, destinationPage, page.Length);
			}

			if (UsedBlocks > other.Blocks.Length)
			{
				other.Blocks = other.Blocks.ResizeToNextPowOf2(UsedBlocks);
			}

			Array.Copy(Blocks, other.Blocks, UsedBlocks);

			other.UsedBlocks = UsedBlocks;
			other.NextFreeBlock = NextFreeBlock;
		}
	}
}
