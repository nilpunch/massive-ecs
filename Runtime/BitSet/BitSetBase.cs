using System;
using System.Runtime.CompilerServices;
using Unity.IL2CPP.CompilerServices;

namespace Massive
{
	[Il2CppSetOption(Option.NullChecks, false)]
	[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
	public class BitSetBase
	{
		public ulong[] Bits { get; protected set; } = Array.Empty<ulong>();
		public ulong[] NonEmptyBlocks { get; protected set; } = Array.Empty<ulong>();
		public ulong[] SaturatedBlocks { get; protected set; } = Array.Empty<ulong>();

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void GrowToFit(BitSetBase other)
		{
			EnsureBlocksCapacity(other.NonEmptyBlocks.Length);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void EnsureBlocksCapacity(int blocksCapacity)
		{
			if (blocksCapacity > NonEmptyBlocks.Length)
			{
				NonEmptyBlocks = NonEmptyBlocks.ResizeToNextPowOf2(blocksCapacity);
				SaturatedBlocks = SaturatedBlocks.Resize(NonEmptyBlocks.Length);
				Bits = Bits.Resize(NonEmptyBlocks.Length << 6);
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void EnsureBlocksCapacityAt(int blockIndex)
		{
			if (blockIndex >= NonEmptyBlocks.Length)
			{
				NonEmptyBlocks = NonEmptyBlocks.ResizeToNextPowOf2(blockIndex + 1);
				SaturatedBlocks = SaturatedBlocks.Resize(NonEmptyBlocks.Length);
				Bits = Bits.Resize(NonEmptyBlocks.Length << 6);
			}
		}

		/// <summary>
		/// Copies bits from this set into the specified one.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void CopyBitsTo(BitSetBase other)
		{
			other.GrowToFit(this);

			Array.Copy(NonEmptyBlocks, other.NonEmptyBlocks, NonEmptyBlocks.Length);
			Array.Copy(SaturatedBlocks, other.SaturatedBlocks, SaturatedBlocks.Length);
			Array.Copy(Bits, other.Bits, Bits.Length);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static BitSetBase GetMinBitSet(BitSetBase[] bitSet)
		{
			var minimal = bitSet[0];
			for (var i = 1; i < bitSet.Length; i++)
			{
				if (minimal.NonEmptyBlocks.Length > bitSet[i].NonEmptyBlocks.Length)
				{
					minimal = bitSet[i];
				}
			}
			return minimal;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static BitSetBase GetMinBitSet(SparseSet[] bitSets, int count)
		{
			var minimal = bitSets[0];
			for (var i = 1; i < count; i++)
			{
				if (minimal.NonEmptyBlocks.Length > bitSets[i].NonEmptyBlocks.Length)
				{
					minimal = bitSets[i];
				}
			}
			return minimal;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static BitSetBase GetMinBitSet(BitSetBase first, BitSetBase[] bitSets)
		{
			var minimal = first;
			for (var i = 0; i < bitSets.Length; i++)
			{
				if (minimal.NonEmptyBlocks.Length > bitSets[i].NonEmptyBlocks.Length)
				{
					minimal = bitSets[i];
				}
			}
			return minimal;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static BitSetBase GetMinBitSet(BitSetBase first, SparseSet[] bitSets, int count)
		{
			var minimal = first;
			for (var i = 0; i < count; i++)
			{
				if (minimal.NonEmptyBlocks.Length > bitSets[i].NonEmptyBlocks.Length)
				{
					minimal = bitSets[i];
				}
			}
			return minimal;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static BitSetBase GetMinBitSet(BitSetBase bitSet1, BitSetBase bitSet2)
		{
			if (bitSet1.NonEmptyBlocks.Length <= bitSet2.NonEmptyBlocks.Length)
			{
				return bitSet1;
			}
			return bitSet2;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static BitSetBase GetMinBitSet(BitSetBase bitSet1, BitSetBase bitSet2, BitSetBase bitSet3)
		{
			if (bitSet1.NonEmptyBlocks.Length <= bitSet2.NonEmptyBlocks.Length && bitSet1.NonEmptyBlocks.Length <= bitSet3.NonEmptyBlocks.Length)
			{
				return bitSet1;
			}
			if (bitSet2.NonEmptyBlocks.Length <= bitSet1.NonEmptyBlocks.Length && bitSet2.NonEmptyBlocks.Length <= bitSet3.NonEmptyBlocks.Length)
			{
				return bitSet2;
			}
			return bitSet3;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static BitSetBase GetMinBitSet(BitSetBase bitSet1, BitSetBase bitSet2, BitSetBase bitSet3, BitSetBase bitSet4)
		{
			if (bitSet1.NonEmptyBlocks.Length <= bitSet2.NonEmptyBlocks.Length && bitSet1.NonEmptyBlocks.Length <= bitSet3.NonEmptyBlocks.Length && bitSet1.NonEmptyBlocks.Length <= bitSet4.NonEmptyBlocks.Length)
			{
				return bitSet1;
			}
			if (bitSet2.NonEmptyBlocks.Length <= bitSet1.NonEmptyBlocks.Length && bitSet2.NonEmptyBlocks.Length <= bitSet3.NonEmptyBlocks.Length && bitSet2.NonEmptyBlocks.Length <= bitSet4.NonEmptyBlocks.Length)
			{
				return bitSet2;
			}
			if (bitSet3.NonEmptyBlocks.Length <= bitSet1.NonEmptyBlocks.Length && bitSet3.NonEmptyBlocks.Length <= bitSet2.NonEmptyBlocks.Length && bitSet3.NonEmptyBlocks.Length <= bitSet4.NonEmptyBlocks.Length)
			{
				return bitSet3;
			}
			return bitSet4;
		}
	}
}
