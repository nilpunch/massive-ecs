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
		private int BitsCapacity { get; set; }

		public int[] NonEmptyBitsIndices { get; private set; } = Array.Empty<int>();
		public int NonEmptyBitsCount { get; private set; }

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

			foreach (var included in Included)
			{
				if (included != minIncluded)
				{
					for (var blockIndex = 0; blockIndex < minBlocksLength; blockIndex++)
					{
						NonEmptyBlocks[blockIndex] &= included.NonEmptyBlocks[blockIndex];
					}
				}
			}

			foreach (var excluded in Excluded)
			{
				excluded.EnsureBlocksCapacity(minBlocksLength);
				for (var blockIndex = 0; blockIndex < minBlocksLength; blockIndex++)
				{
					NonEmptyBlocks[blockIndex] &= ~excluded.SaturatedBlocks[blockIndex];
				}
			}

			if (minBlocksLength << 6 > NonEmptyBitsIndices.Length)
			{
				NonEmptyBitsIndices = NonEmptyBitsIndices.ResizeToNextPowOf2(minBlocksLength << 6);
			}

			NonEmptyBitsCount = 0;
			var deBruijn = MathUtils.DeBruijn;
			for (var blockIndex = 0; blockIndex < minBlocksLength; blockIndex++)
			{
				var block = NonEmptyBlocks[blockIndex];
				if (block == 0UL)
				{
					continue;
				}

				var blockOffset = blockIndex << 6;

				var blockBit = (int)deBruijn[(int)(((block & (ulong)-(long)block) * 0x37E84A99DAE458FUL) >> 58)];

				var runEnd = MathUtils.ApproximateMSB(block);
				var setBits = MathUtils.PopCount(block);
				if (setBits << 1 > runEnd - blockBit)
				{
					for (; blockBit < runEnd; blockBit++)
					{
						if ((block & (1UL << blockBit)) == 0UL)
						{
							continue;
						}

						NonEmptyBitsIndices[NonEmptyBitsCount++] = blockOffset + blockBit;
					}
				}
				else
				{
					do
					{
						NonEmptyBitsIndices[NonEmptyBitsCount++] = blockOffset + blockBit;
						block &= block - 1UL;
						blockBit = deBruijn[(int)(((block & (ulong)-(long)block) * 0x37E84A99DAE458FUL) >> 58)];
					} while (block != 0UL);
				}
			}

			for (var i = 0; i < NonEmptyBitsCount; i++)
			{
				var bitsIndex = NonEmptyBitsIndices[i];
				Bits[bitsIndex] = minIncluded.Bits[bitsIndex];
			}

			foreach (var included in Included)
			{
				if (included != minIncluded)
				{
					for (var i = 0; i < NonEmptyBitsCount; i++)
					{
						var bitsIndex = NonEmptyBitsIndices[i];
						Bits[bitsIndex] &= included.Bits[bitsIndex];
					}
				}
			}

			foreach (var excluded in Excluded)
			{
				for (var i = 0; i < NonEmptyBitsCount; i++)
				{
					var bitsIndex = NonEmptyBitsIndices[i];
					Bits[bitsIndex] &= ~excluded.Bits[bitsIndex];
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
				BitsCapacity = NonEmptyBlocks.Length << 6;
				Bits = Bits.Resize(BitsCapacity);
			}
		}
	}
}
