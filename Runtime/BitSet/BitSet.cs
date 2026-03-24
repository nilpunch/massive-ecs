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
	public partial class BitSet : BitSetBase
	{
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

			var bitsMask = 1UL << (id & 63);
			var blockMask = 1UL << (bitsIndex & 63);

			if ((Bits[bitsIndex] & bitsMask) != 0UL)
			{
				return false;
			}

			if (Bits[bitsIndex] == 0UL)
			{
				EnsurePage(id >> Constants.PageSizePower);
				NonEmptyBlocks[blockIndex] |= blockMask;
			}
			Bits[bitsIndex] |= bitsMask;
			if (Bits[bitsIndex] == ulong.MaxValue)
			{
				SaturatedBlocks[blockIndex] |= blockMask;
			}

			NotifyAfterAdded(id);

			for (var i = 0; i < RemoveOnAddCount; i++)
			{
				RemoveOnAdd[i].RemoveBit(bitsIndex, bitsMask);
			}

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
			var bitsMask = 1UL << (id & 63);
			var blockMask = 1UL << (bitsIndex & 63);

			if (blockIndex >= BlocksCapacity || (Bits[bitsIndex] & bitsMask) == 0UL)
			{
				return false;
			}

			BeforeRemoved?.Invoke(id);
			if (IsComponentBound)
			{
				Components.BitMap[id * Components.MaskLength + ComponentIndex] &= ComponentMaskNegative;
			}
			else if (Components != null)
			{
				Sets.EnsureBinded(this);
				Components.BitMap[id * Components.MaskLength + ComponentIndex] &= ComponentMaskNegative;
			}

			ClearData(id);

			if (Bits[bitsIndex] == ulong.MaxValue)
			{
				SaturatedBlocks[blockIndex] &= ~blockMask;
			}
			Bits[bitsIndex] &= ~bitsMask;
			if (Bits[bitsIndex] == 0UL)
			{
				NonEmptyBlocks[blockIndex] &= ~blockMask;

				var pageIndex = id >> Constants.PageSizePower;
				if ((NonEmptyBlocks[blockIndex] & PageMasks[pageIndex & Constants.PagesInBlockMinusOne]) == 0UL)
				{
					FreePage(pageIndex);
				}
			}

			for (var i = 0; i < RemoveOnRemoveCount; i++)
			{
				RemoveOnRemove[i].RemoveBit(bitsIndex, bitsMask);
			}

			return true;
		}

		/// <summary>
		/// Removes all IDs and triggers the <see cref="BeforeRemoved"/> event for each one.
		/// </summary>
		public void Clear()
		{
			var blocksLength = BlocksCapacity;

			var deBruijn = MathUtils.DeBruijn;
			for (var blockIndex = 0; blockIndex < blocksLength; blockIndex++)
			{
				var block = NonEmptyBlocks[blockIndex];
				var blockOffset = blockIndex << 6;

				// Block loop.
				while (block != 0)
				{
					var blockBit = block & (ulong)-(long)block;
					var blockBitIndex = deBruijn[(uint)((blockBit * 0x37E84A99DAE458FUL) >> 58)];
					var bitsIndex = blockOffset + blockBitIndex;

					do
					{
						var bits = Bits[bitsIndex];
						var bitsOffset = bitsIndex << 6;

						// Bits loop.
						do
						{
							var bitsBit = bits & (ulong)-(long)bits;
							var bitsBitIndex = deBruijn[(uint)((bitsBit * 0x37E84A99DAE458FUL) >> 58)];
							var entityId = bitsOffset + bitsBitIndex;

							do
							{
								BeforeRemoved?.Invoke(entityId);
								if (IsComponentBound)
								{
									Components.BitMap[entityId * Components.MaskLength + ComponentIndex] &= ComponentMaskNegative;
								}
								else if (Components != null)
								{
									Sets.EnsureBinded(this);
									Components.BitMap[entityId * Components.MaskLength + ComponentIndex] &= ComponentMaskNegative;
								}
								ClearData(entityId);
								for (var i = 0; i < RemoveOnRemoveCount; i++)
								{
									RemoveOnRemove[i].RemoveBit(bitsIndex, bitsBit);
								}

								bitsBit <<= 1;
								entityId++;
							} while ((bits & bitsBit) != 0);

							bits &= ~(bitsBit - 1);
						} while (bits != 0);

						blockBit <<= 1;
						bitsIndex++;
					} while ((block & blockBit) != 0);

					Bits[bitsIndex] = 0UL;

					block &= ~(blockBit - 1);
				}

				NonEmptyBlocks[blockIndex] = 0UL;
				SaturatedBlocks[blockIndex] = 0UL;
			}

			FreeAllPages();
		}

		/// <summary>
		/// Removes all IDs without triggering anything.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Reset()
		{
			FillPagesWithDefaultValue();
			Array.Fill(NonEmptyBlocks, 0UL);
			Array.Fill(SaturatedBlocks, 0UL);
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
			var cache = QueryCache.Rent().AddInclude(this).Update();
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

		protected virtual void ClearData(int id)
		{
		}

		protected virtual void FillPagesWithDefaultValue()
		{
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		protected void NotifyAfterAdded(int id)
		{
			if (IsComponentBound)
			{
				Components.BitMap[id * Components.MaskLength + ComponentIndex] |= ComponentMask;
			}
			else if (Components != null)
			{
				Sets.EnsureBinded(this);
				Components.BitMap[id * Components.MaskLength + ComponentIndex] |= ComponentMask;
			}
			AfterAdded?.Invoke(id);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public BitSet CloneBitSet()
		{
			var clone = new BitSet();
			CopyBitSetTo(clone);
			return clone;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void CopyBitSetTo(BitSet other)
		{
			if (IsComponentBound && other.Components != null)
			{
				other.Sets.EnsureBinded(other);
			}

			other.GrowToFit(this);

			Array.Copy(NonEmptyBlocks, other.NonEmptyBlocks, BlocksCapacity);
			Array.Copy(SaturatedBlocks, other.SaturatedBlocks, BlocksCapacity);
			Array.Copy(Bits, other.Bits, Bits.Length);
		}
	}
}
