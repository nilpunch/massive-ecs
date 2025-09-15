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
			var id0 = id >> 6;
			var id1 = id >> 12;

			if (id1 >= NonEmptyBlocks.Length)
			{
				NonEmptyBlocks = NonEmptyBlocks.Resize(MathUtils.NextPowerOf2(id1 + 1));
				Bits = Bits.Resize(NonEmptyBlocks.Length << 6);
			}

			var bit0 = 1UL << (id & 63);
			var bit1 = 1UL << (id0 & 63);

			if (Bits[id0] == 0UL)
			{
				NonEmptyBlocks[id1] |= bit1;
			}
			Bits[id0] |= bit0;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Remove(int id)
		{
			var id0 = id >> 6;
			var id1 = id >> 12;

			if (id0 >= Bits.Length)
			{
				return;
			}

			var bit0 = 1UL << (id & 63);
			var bit1 = 1UL << (id0 & 63);

			Bits[id0] &= ~bit0;
			if (Bits[id0] == 0UL)
			{
				NonEmptyBlocks[id1] &= ~bit1;
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool Has(int id)
		{
			var id0 = id >> 6;

			if (id < 0 || id0 >= Bits.Length)
			{
				return false;
			}

			var bit0 = 1UL << (id & 63);
			return (Bits[id0] & bit0) != 0UL;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public BitSet AndBits(BitSetBase other)
		{
			var minBits = GetMinBitSet(this, other);

			if (NonEmptyBlocks.Length > other.NonEmptyBlocks.Length)
			{
				Array.Fill(NonEmptyBlocks, 0UL, other.NonEmptyBlocks.Length, NonEmptyBlocks.Length - other.NonEmptyBlocks.Length);
				Array.Fill(Bits, 0UL, other.Bits.Length, Bits.Length - other.Bits.Length);
			}

			for (var i = 0; i < minBits.NonEmptyBlocks.Length; i++)
			{
				NonEmptyBlocks[i] &= other.NonEmptyBlocks[i];
			}

			for (var j = 0; j < minBits.Bits.Length; j++)
			{
				Bits[j] &= other.Bits[j];
			}

			return this;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public BitSet NotBits(BitSetBase other)
		{
			var minBits = GetMinBitSet(this, other);

			for (var i = 0; i < minBits.NonEmptyBlocks.Length; i++)
			{
				NonEmptyBlocks[i] &= ~other.NonEmptyBlocks[i];
			}

			for (var j = 0; j < minBits.Bits.Length; j++)
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
