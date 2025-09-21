#if !MASSIVE_DISABLE_ASSERT
#define MASSIVE_ASSERT
#endif

using System;
using System.Runtime.CompilerServices;
using Unity.IL2CPP.CompilerServices;

namespace Massive
{
	[Il2CppSetOption(Option.NullChecks, false)]
	[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
	public class BitSet : BitSetBase
	{
		/// <summary>
		/// Associated component index. Session-dependent, used for lookups.<br/>
		/// </summary>
		public int ComponentId { get; set; } = -1;

		/// <summary>
		/// Shortcut to access components.
		/// </summary>
		public Components Components { get; set; }

		/// <summary>
		/// Shoots only after <see cref="Add"/> call, when the ID was not already present.
		/// </summary>
		public event Action<int> AfterAdded;

		/// <summary>
		/// Shoots before each <see cref="Remove"/> call, when the ID was removed.
		/// </summary>
		public event Action<int> BeforeRemoved;

		/// <summary>
		/// Adds an ID. If the ID is already added, no action is performed.
		/// </summary>
		/// <returns>
		/// True if the ID was added; false if it was already present.
		/// </returns>
		/// <remarks>
		/// Throws if provided ID is negative.
		/// </remarks>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool Add(int id)
		{
			NegativeArgumentException.ThrowIfNegative(id);

			var bitsIndex = id >> 6;
			var blockIndex = id >> 12;

			EnsureBlocksCapacityAt(blockIndex);

			var bitsBit = 1UL << (id & 63);
			var blockBit = 1UL << (bitsIndex & 63);

			if ((Bits[bitsIndex] & bitsBit) != 0UL)
			{
				return false;
			}

			if (Bits[bitsIndex] == 0UL)
			{
				EnsurePage(id >> Constants.PageSizePower);
				NonEmptyBlocks[blockIndex] |= blockBit;
			}
			Bits[bitsIndex] |= bitsBit;
			if (Bits[bitsIndex] == ulong.MaxValue)
			{
				SaturatedBlocks[blockIndex] |= blockBit;
			}

			PrepareData(id);

			for (var i = 0; i < RemoveOnAddCount; i++)
			{
				RemoveOnAdd[i].RemoveBit(bitsIndex, bitsBit);
			}

			Components?.Set(id, ComponentId);
			AfterAdded?.Invoke(id);

			return true;
		}

		/// <summary>
		/// Removes an ID. If the ID is already removed, no action is performed.
		/// </summary>
		/// <returns>
		/// True if the ID was removed; false if it was not present.
		/// </returns>
		/// <remarks>
		/// Throws if provided ID is negative.
		/// </remarks>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool Remove(int id)
		{
			NegativeArgumentException.ThrowIfNegative(id);

			var bitsIndex = id >> 6;
			var blockIndex = id >> 12;

			if (bitsIndex >= Bits.Length)
			{
				return false;
			}

			var bitsBit = 1UL << (id & 63);
			var blockBit = 1UL << (bitsIndex & 63);

			if ((Bits[bitsIndex] & bitsBit) == 0UL)
			{
				return false;
			}

			BeforeRemoved?.Invoke(id);
			Components?.Remove(id, ComponentId);

			if (Bits[bitsIndex] == ulong.MaxValue)
			{
				SaturatedBlocks[blockIndex] &= ~blockBit;
			}
			Bits[bitsIndex] &= ~bitsBit;
			if (Bits[bitsIndex] == 0UL)
			{
				NonEmptyBlocks[blockIndex] &= ~blockBit;

				var pageIndex = id >> Constants.PageSizePower;
				var pageMask = Constants.PageMask << ((pageIndex & Constants.PagesInBlockMinusOne) << Constants.PageMaskShift);
				if ((NonEmptyBlocks[blockIndex] & pageMask) == 0UL)
				{
					FreePage(pageIndex);
				}
			}

			for (var i = 0; i < RemoveOnRemoveCount; i++)
			{
				RemoveOnRemove[i].RemoveBit(bitsIndex, bitsBit);
			}

			return true;
		}

		/// <summary>
		/// Removes all IDs and triggers the <see cref="BeforeRemoved"/> event for each one.
		/// </summary>
		public void Clear()
		{
			var blocksLength = NonEmptyBlocks.Length;

			var deBruijn = MathUtils.DeBruijn;
			for (var blockIndex = 0; blockIndex < blocksLength; blockIndex++)
			{
				var block = NonEmptyBlocks[blockIndex];
				var blockOffset = blockIndex << 6;
				while (block != 0UL)
				{
					var blockBit = (int)deBruijn[(int)(((block & (ulong)-(long)block) * 0x37E84A99DAE458FUL) >> 58)];

					var bitsIndex = blockOffset + blockBit;
					var bits = Bits[bitsIndex];
					var bitsOffset = bitsIndex << 6;

					while (bits != 0UL)
					{
						var bit = (int)deBruijn[(int)(((bits & (ulong)-(long)bits) * 0x37E84A99DAE458FUL) >> 58)];

						var id = bitsOffset + bit;
						BeforeRemoved?.Invoke(id);
						Components?.Remove(id, ComponentId);

						for (var i = 0; i < RemoveOnRemoveCount; i++)
						{
							RemoveOnRemove[i].RemoveBit(bitsIndex, 1UL << bit);
						}

						bits &= bits - 1UL;
					}

					block &= block - 1UL;

					Bits[bitsIndex] = 0UL;
				}
				NonEmptyBlocks[blockIndex] = 0UL;
				SaturatedBlocks[blockIndex] = 0UL;
			}

			FreeAllPages();
		}

		/// <summary>
		/// Removes all IDs without triggering the <see cref="BeforeRemoved"/> event.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void ClearWithoutNotify()
		{
			Array.Fill(NonEmptyBlocks, 0UL);
			Array.Fill(Bits, 0UL);
			FreeAllPages();
		}

		/// <summary>
		/// Checks whether the specified ID is present.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool Has(int id)
		{
			var bitsIndex = id >> 6;

			if (bitsIndex < 0 || bitsIndex >= Bits.Length)
			{
				return false;
			}

			var bit = 1UL << (id & 63);
			return (Bits[bitsIndex] & bit) != 0UL;
		}

		public IdsEnumerator GetEnumerator()
		{
			var cache = QueryCache.Rent().AddToAll(this).Update();
			return new IdsEnumerator(cache);
		}

		public virtual void CopyData(int sourceId, int destinationId)
		{
		}

		public virtual void EnsurePage(int page)
		{
		}

		protected virtual void FreePage(int page)
		{
		}

		protected virtual void FreeAllPages()
		{
		}

		protected virtual void PrepareData(int id)
		{
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		protected void NotifyAfterAdded(int id)
		{
			AfterAdded?.Invoke(id);
		}

		/// <summary>
		/// Creates and returns a new sparse set that is an exact copy of this one.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public BitSet CloneBits()
		{
			var clone = new BitSet();
			CopyBitsTo(clone);
			return clone;
		}
	}
}
