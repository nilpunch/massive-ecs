using System;
using System.Collections.Generic;
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

		private FastList<BitSetBase> Included { get; } = new FastList<BitSetBase>();
		private FastList<BitSetBase> Excluded { get; } = new FastList<BitSetBase>();
		private FastList<BitSetBase> IncludedWithoutMin { get; } = new FastList<BitSetBase>();

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
			queryCache.PopAll();
		}

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
		public void PopAll()
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
				Bits = Bits.Resize(NonEmptyBlocks.Length << 6);
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void EnsureBlocksCapacityAt(int blockIndex)
		{
			if (blockIndex >= NonEmptyBlocks.Length)
			{
				NonEmptyBlocks = NonEmptyBlocks.ResizeToNextPowOf2(blockIndex + 1);
				Bits = Bits.Resize(NonEmptyBlocks.Length << 6);
			}
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
		public QueryCache Update()
		{
			// There always must be at least 1 included.
			var minIncluded = BitSetBase.GetMinBitSet(Included.Items, Included.Count);
			var minBlocksLength = minIncluded.NonEmptyBlocks.Length;

			GrowToFit(minIncluded);
			Array.Copy(minIncluded.NonEmptyBlocks, NonEmptyBlocks, minBlocksLength);

			foreach (var included in Included)
			{
				if (included != minIncluded)
				{
					IncludedWithoutMin.Add(included);
				}
			}

			foreach (var included in IncludedWithoutMin)
			{
				for (var blockIndex = 0; blockIndex < minBlocksLength; blockIndex++)
				{
					NonEmptyBlocks[blockIndex] &= included.NonEmptyBlocks[blockIndex];
				}
			}

			foreach (var excluded in Excluded)
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
				while (block != 0UL)
				{
					var blockBit = deBruijn[(int)(((block & (ulong)-(long)block) * 0x37E84A99DAE458FUL) >> 58)];
					block &= block - 1UL;
					NonEmptyBitsIndices[NonEmptyBitsCount++] = blockOffset + blockBit;
				}
			}

			for (var i = 0; i < NonEmptyBitsCount; i++)
			{
				var bitsIndex = NonEmptyBitsIndices[i];
				Bits[bitsIndex] = minIncluded.Bits[bitsIndex];
			}

			foreach (var included in IncludedWithoutMin)
			{
				for (var i = 0; i < NonEmptyBitsCount; i++)
				{
					var bitsIndex = NonEmptyBitsIndices[i];
					Bits[bitsIndex] &= included.Bits[bitsIndex];
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

			IncludedWithoutMin.Clear();
			return this;
		}
	}
}
