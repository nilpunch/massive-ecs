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
	public class QueryCache
	{
		public static QueryCache[] CachePool { get; set; } = Array.Empty<QueryCache>();
		public static int PoolCount { get; set; }

		public ulong[] Bits { get; private set; } = Array.Empty<ulong>();
		public ulong[] NonEmptyBlocks { get; private set; } = Array.Empty<ulong>();

		public int[] NonEmptyBitsIndices { get; private set; } = Array.Empty<int>();
		public int NonEmptyBitsCount { get; private set; }

		private FastList<BitSetBase> All { get; } = new FastList<BitSetBase>();
		private FastList<BitSetBase> None { get; } = new FastList<BitSetBase>();
		private FastList<BitSetBase> AllWithoutMin { get; } = new FastList<BitSetBase>();

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static QueryCache Rent()
		{
			if (PoolCount > 0)
			{
				return CachePool[--PoolCount];
			}

			return new QueryCache();
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void ReturnAndPop(QueryCache queryCache)
		{
			if (PoolCount >= CachePool.Length)
			{
				CachePool = CachePool.Resize(MathUtils.NextPowerOf2(PoolCount + 1));
			}

			CachePool[PoolCount++] = queryCache;
			queryCache.Pop();
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public QueryCache AddToAll(BitSetBase bitSet)
		{
			All.Add(bitSet);
			bitSet.PushRemoveOnRemove(this);
			return this;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public QueryCache AddToNone(BitSetBase bitSet)
		{
			None.Add(bitSet);
			bitSet.PushRemoveOnAdd(this);
			return this;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Pop()
		{
			foreach (var bitSet in None)
			{
				bitSet.PopRemoveOnAdd();
			}
			foreach (var bitSet in All)
			{
				bitSet.PopRemoveOnRemove();
			}
			None.Clear();
			All.Clear();
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void RemoveBit(int bitsIndex, ulong bitsBit)
		{
			if (bitsIndex >= Bits.Length)
			{
				return;
			}

			Bits[bitsIndex] &= ~bitsBit;
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public QueryCache Update()
		{
			var minIncluded = BitSetBase.GetMinBitSet(All.Items, All.Count);
			var minBlocksLength = minIncluded.NonEmptyBlocks.Length;

			EnsureBlocksCapacity(minBlocksLength);
			Array.Copy(minIncluded.NonEmptyBlocks, NonEmptyBlocks, minBlocksLength);

			foreach (var included in All)
			{
				if (included != minIncluded)
				{
					AllWithoutMin.Add(included);
				}
			}

			foreach (var included in AllWithoutMin)
			{
				for (var blockIndex = 0; blockIndex < minBlocksLength; blockIndex++)
				{
					NonEmptyBlocks[blockIndex] &= included.NonEmptyBlocks[blockIndex];
				}
			}

			foreach (var excluded in None)
			{
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
						if ((NonEmptyBlocks[blockIndex] & (1UL << blockBit)) == 0UL)
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

			foreach (var included in AllWithoutMin)
			{
				for (var i = 0; i < NonEmptyBitsCount; i++)
				{
					var bitsIndex = NonEmptyBitsIndices[i];
					Bits[bitsIndex] &= included.Bits[bitsIndex];
				}
			}

			foreach (var excluded in None)
			{
				for (var i = 0; i < NonEmptyBitsCount; i++)
				{
					var bitsIndex = NonEmptyBitsIndices[i];
					Bits[bitsIndex] &= ~excluded.Bits[bitsIndex];
				}
			}

			AllWithoutMin.Clear();
			return this;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void EnsureBlocksCapacity(int blocksCapacity)
		{
			if (blocksCapacity > NonEmptyBlocks.Length)
			{
				NonEmptyBlocks = NonEmptyBlocks.ResizeToNextPowOf2(blocksCapacity);
				Bits = Bits.Resize(NonEmptyBlocks.Length << 6);
			}
		}
	}
}
