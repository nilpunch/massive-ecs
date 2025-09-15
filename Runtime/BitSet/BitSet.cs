using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.IL2CPP.CompilerServices;

namespace Massive
{
	[Il2CppSetOption(Option.NullChecks, false)]
	[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
	public class BitSet : BitSetBase
	{
		private List<IBitSetObservable> PopOnAdd { get; } = new List<IBitSetObservable>();
		private List<IBitSetObservable> PopOnRemove { get; } = new List<IBitSetObservable>();

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public BitSet RemoveOnAdd(IBitSetObservable bits)
		{
			bits.PushRemoveOnAdd(this);
			PopOnAdd.Add(bits);
			return this;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public BitSet RemoveOnRemove(IBitSetObservable bits)
		{
			bits.PushRemoveOnRemove(this);
			PopOnRemove.Add(bits);
			return this;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void PopAll()
		{
			foreach (var bitSet in PopOnAdd)
			{
				bitSet.PopRemoveOnAdd();
			}
			foreach (var bitSet in PopOnRemove)
			{
				bitSet.PopRemoveOnRemove();
			}
			PopOnAdd.Clear();
			PopOnRemove.Clear();
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Add(int id)
		{
			var bitsIndex = id >> 6;
			var blockIndex = id >> 12;

			EnsureBlocksCapacityAt(blockIndex);

			var bitsBit = 1UL << (id & 63);
			var blockBit = 1UL << (bitsIndex & 63);

			if (Bits[bitsIndex] == 0UL)
			{
				NonEmptyBlocks[blockIndex] |= blockBit;
			}
			Bits[bitsIndex] |= bitsBit;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Remove(int id)
		{
			var bitsIndex = id >> 6;
			var blockIndex = id >> 12;

			if (bitsIndex >= Bits.Length)
			{
				return;
			}

			var bitsBit = 1UL << (id & 63);
			var blockBit = 1UL << (bitsIndex & 63);

			Bits[bitsIndex] &= ~bitsBit;
			if (Bits[bitsIndex] == 0UL)
			{
				NonEmptyBlocks[blockIndex] &= ~blockBit;
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool Has(int id)
		{
			var bitsIndex = id >> 6;

			if (id < 0 || bitsIndex >= Bits.Length)
			{
				return false;
			}

			var bit = 1UL << (id & 63);
			return (Bits[bitsIndex] & bit) != 0UL;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public BitSet AndBits(BitSetBase other)
		{
			var minBitSet = GetMinBitSet(this, other);

			if (NonEmptyBlocks.Length > other.NonEmptyBlocks.Length)
			{
				Array.Fill(NonEmptyBlocks, 0UL, other.NonEmptyBlocks.Length, NonEmptyBlocks.Length - other.NonEmptyBlocks.Length);
				Array.Fill(Bits, 0UL, other.Bits.Length, Bits.Length - other.Bits.Length);
			}

			for (var i = 0; i < minBitSet.NonEmptyBlocks.Length; i++)
			{
				NonEmptyBlocks[i] &= other.NonEmptyBlocks[i];
			}

			for (var j = 0; j < minBitSet.Bits.Length; j++)
			{
				Bits[j] &= other.Bits[j];
			}

			return this;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public BitSet NotBits(BitSetBase other)
		{
			var minBitSet = GetMinBitSet(this, other);

			for (var i = 0; i < minBitSet.NonEmptyBlocks.Length; i++)
			{
				NonEmptyBlocks[i] &= ~other.NonEmptyBlocks[i];
			}

			for (var j = 0; j < minBitSet.Bits.Length; j++)
			{
				Bits[j] &= ~other.Bits[j];
			}

			return this;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public BitSet OrBits(BitSetBase other)
		{
			GrowToFit(other);

			for (var i = 0; i < other.NonEmptyBlocks.Length; i++)
			{
				NonEmptyBlocks[i] |= other.NonEmptyBlocks[i];
			}

			for (var j = 0; j < other.Bits.Length; j++)
			{
				Bits[j] |= other.Bits[j];
			}

			return this;
		}
	}
}
