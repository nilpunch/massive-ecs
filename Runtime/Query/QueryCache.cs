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
	[Il2CppEagerStaticClassConstruction]
	public partial class QueryCache
	{
		public ulong[] Bits { get; private set; } = new ulong[1];
		private ulong[] NonEmptyBlocks { get; set; } = Array.Empty<ulong>();
		private ulong[] SaturatedBlocks { get; set; } = Array.Empty<ulong>();
		private int BitsCapacity { get; set; }

		public int[] NonEmptyBitsIndices { get; private set; } = Array.Empty<int>();
		private int[] SaturatedBitsIndices { get; set; } = Array.Empty<int>();
		private int[] RefineBitsIndices { get; set; } = Array.Empty<int>();
		public int NonEmptyBitsCount { get; private set; }
		public int SaturatedBitsCount { get; private set; }
		public int RefineBitsCount { get; private set; }

		private FastList<BitSetBase> Included { get; } = new FastList<BitSetBase>();
		private FastList<BitSetBase> Excluded { get; } = new FastList<BitSetBase>();

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public QueryCache AddInclude(BitSetBase bitSet)
		{
			Included.Add(bitSet);
			bitSet.PushRemoveOnRemove(this);
			return this;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public QueryCache AddExclude(BitSetBase bitSet)
		{
			Excluded.Add(bitSet);
			bitSet.PushRemoveOnAdd(this);
			return this;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Pop()
		{
			foreach (var bitSet in Excluded)
			{
				bitSet.PopRemoveOnAdd();
			}
			foreach (var bitSet in Included)
			{
				bitSet.PopRemoveOnRemove();
			}
			Excluded.Clear();
			Included.Clear();
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void RemoveBit(int bitsIndex, ulong bitsMask)
		{
			if (bitsIndex >= BitsCapacity)
			{
				return;
			}

			Bits[bitsIndex] &= ~bitsMask;
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public QueryCache Update()
		{
			var minIncluded = BitSetBase.GetMinBitSet(Included.Items, Included.Count);
			var minBlocksLength = minIncluded.BlocksCapacity;

			EnsureBlocksCapacity(minBlocksLength);
			Array.Copy(minIncluded.NonEmptyBlocks, NonEmptyBlocks, minBlocksLength);
			Array.Copy(minIncluded.SaturatedBlocks, SaturatedBlocks, minBlocksLength);

			foreach (var included in Included)
			{
				if (included != minIncluded)
				{
					var includedNonEmptyBlocks = included.NonEmptyBlocks;
					var includedSaturatedBlocks = included.SaturatedBlocks;

					for (var blockIndex = 0; blockIndex < minBlocksLength; blockIndex++)
					{
						NonEmptyBlocks[blockIndex] &= includedNonEmptyBlocks[blockIndex];
						SaturatedBlocks[blockIndex] &= includedSaturatedBlocks[blockIndex];
					}
				}
			}

			foreach (var excluded in Excluded)
			{
				excluded.EnsureBlocksCapacity(minBlocksLength);

				var excludedNonEmptyBlocks = excluded.NonEmptyBlocks;
				var excludedSaturatedBlocks = excluded.SaturatedBlocks;

				for (var blockIndex = 0; blockIndex < minBlocksLength; blockIndex++)
				{
					NonEmptyBlocks[blockIndex] &= ~excludedSaturatedBlocks[blockIndex];
					SaturatedBlocks[blockIndex] &= ~excludedNonEmptyBlocks[blockIndex];
				}
			}

			if (minBlocksLength << 6 > NonEmptyBitsIndices.Length)
			{
				NonEmptyBitsIndices = NonEmptyBitsIndices.ResizeToNextPowOf2(minBlocksLength << 6);
				SaturatedBitsIndices = SaturatedBitsIndices.ResizeToNextPowOf2(minBlocksLength << 6);
				RefineBitsIndices = RefineBitsIndices.ResizeToNextPowOf2(minBlocksLength << 6);
			}

			NonEmptyBitsCount = 0;
			SaturatedBitsCount = 0;
			RefineBitsCount = 0;
			var deBruijn = MathUtils.DeBruijn;
			for (var blockIndex = 0; blockIndex < minBlocksLength; blockIndex++)
			{
				var nonEmptyBlock = NonEmptyBlocks[blockIndex];
				var saturatedBlock = SaturatedBlocks[blockIndex];

				var blockOffset = blockIndex << 6;

				while (nonEmptyBlock != 0)
				{
					var isolatedBit = nonEmptyBlock & (ulong)-(long)nonEmptyBlock;
					var blockBitIndex = deBruijn[(uint)((isolatedBit * 0x37E84A99DAE458FUL) >> 58)];

					do
					{
						var bitsIndex = blockOffset + blockBitIndex;

						NonEmptyBitsIndices[NonEmptyBitsCount++] = bitsIndex;

						var oneIfSaturated = (int)((saturatedBlock >> blockBitIndex) & 1UL);
						SaturatedBitsIndices[SaturatedBitsCount] = bitsIndex;
						RefineBitsIndices[RefineBitsCount] = bitsIndex;
						SaturatedBitsCount += oneIfSaturated;
						RefineBitsCount += 1 - oneIfSaturated;

						isolatedBit <<= 1;
						blockBitIndex++;
					} while ((nonEmptyBlock & isolatedBit) != 0);

					nonEmptyBlock &= ~(isolatedBit - 1);
				}
			}

			for (var i = 0; i < SaturatedBitsCount; i++)
			{
				var bitsIndex = SaturatedBitsIndices[i];
				Bits[bitsIndex] = ulong.MaxValue;
			}

			var minIncludedBits = minIncluded.Bits;
			for (var i = 0; i < RefineBitsCount; i++)
			{
				var bitsIndex = RefineBitsIndices[i];
				Bits[bitsIndex] = minIncludedBits[bitsIndex];
			}

			foreach (var included in Included)
			{
				if (included != minIncluded)
				{
					var includedBits = included.Bits;
					for (var i = 0; i < RefineBitsCount; i++)
					{
						var bitsIndex = RefineBitsIndices[i];
						Bits[bitsIndex] &= includedBits[bitsIndex];
					}
				}
			}

			foreach (var excluded in Excluded)
			{
				var excludedBits = excluded.Bits;
				for (var i = 0; i < RefineBitsCount; i++)
				{
					var bitsIndex = RefineBitsIndices[i];
					Bits[bitsIndex] &= ~excludedBits[bitsIndex];
				}
			}

			return this;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void EnsureBlocksCapacity(int blocksCapacity)
		{
			if (blocksCapacity > NonEmptyBlocks.Length)
			{
				NonEmptyBlocks = NonEmptyBlocks.ResizeToNextPowOf2(blocksCapacity);
				SaturatedBlocks = SaturatedBlocks.Resize(NonEmptyBlocks.Length);
				BitsCapacity = NonEmptyBlocks.Length << 6;
				Bits = Bits.Resize(BitsCapacity);
			}
		}
	}
}
