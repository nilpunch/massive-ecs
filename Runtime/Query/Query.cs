using Unity.IL2CPP.CompilerServices;

namespace Massive
{
	[Il2CppSetOption(Option.NullChecks, false)]
	[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
	public static class Query
	{
		public struct Context
		{
			public World World;
			public Filter Filter;
		}

		public static void ForEach<TAction>(this Context context, ref TAction action)
			where TAction : IEntityAction
		{
			var minBitSet = BitSetBase.GetMinBitSet(context.World.Entifiers, context.Filter.Included, context.Filter.IncludedCount);

			var resultBitSet = BitsPool.RentClone(minBitSet);

			if (minBitSet == context.World.Entifiers)
			{
				resultBitSet.RemoveOnRemove(context.World.Entifiers);
			}

			ApplyFilter(context.Filter, resultBitSet);

			var blocksLength = minBitSet.NonEmptyBlocks.Length;

			var deBruijn = MathUtils.DeBruijn;
			for (var blockIndex = 0; blockIndex < blocksLength; blockIndex++)
			{
				var block = resultBitSet.NonEmptyBlocks[blockIndex];
				var blockOffset = blockIndex << 6;
				while (block != 0UL)
				{
					var blockBit = deBruijn[(int)(((block & (ulong)-(long)block) * 0x37E84A99DAE458FUL) >> 58)];

					var bitsIndex = blockOffset + blockBit;
					var bits = resultBitSet.Bits[bitsIndex];
					var bitsOffset = bitsIndex << 6;
					var bit = deBruijn[(int)(((bits & (ulong)-(long)bits) * 0x37E84A99DAE458FUL) >> 58)];

					var runEnd = MathUtils.ApproximateMSB(bits);
					var setBits = MathUtils.PopCount(bits);
					if (setBits << 1 > runEnd - bit)
					{
						for (; bit < runEnd; bit++)
						{
							if ((resultBitSet.Bits[bitsIndex] & (1UL << bit)) == 0UL)
							{
								continue;
							}

							action.Apply(bitsOffset + bit);
						}
					}
					else
					{
						do
						{
							action.Apply(bitsOffset + bit);
							bits &= (bits - 1UL) & resultBitSet.Bits[bitsIndex];
							bit = deBruijn[(int)(((bits & (ulong)-(long)bits) * 0x37E84A99DAE458FUL) >> 58)];
						} while (bits != 0UL);
					}

					block &= (block - 1UL) & resultBitSet.NonEmptyBlocks[blockIndex];
				}
			}

			BitsPool.ReturnAndPop(resultBitSet);
		}

		public static void ForEach<T, TAction>(this Context context, ref TAction action)
			where TAction : IEntityAction<T>
		{
			NoDataException.ThrowIfHasNoData<T>(context.World, DataAccessContext.View);

			var dataSet1 = context.World.DataSet<T>();

			var resultBitSet = BitsPool.RentClone(dataSet1).RemoveOnRemove(dataSet1);

			ApplyFilter(context.Filter, resultBitSet);

			var blocksLength = dataSet1.NonEmptyBlocks.Length;

			var deBruijn = MathUtils.DeBruijn;
			for (var blockIndex = 0; blockIndex < blocksLength; blockIndex++)
			{
				var block = resultBitSet.NonEmptyBlocks[blockIndex];
				var blockOffset = blockIndex << 6;
				while (block != 0UL)
				{
					var blockBit = deBruijn[(int)(((block & (ulong)-(long)block) * 0x37E84A99DAE458FUL) >> 58)];

					var bitsIndex = blockOffset + blockBit;
					var bits = resultBitSet.Bits[bitsIndex];
					var bitsOffset = bitsIndex << 6;
					var bit = deBruijn[(int)(((bits & (ulong)-(long)bits) * 0x37E84A99DAE458FUL) >> 58)];
					var dataOffset = bitsOffset & Constants.PageSizeMinusOne;
					var pageIndex = bitsOffset >> Constants.PageSizePower;
					var dataPage1 = dataSet1.PagedData[pageIndex];

					var runEnd = MathUtils.ApproximateMSB(bits);
					var setBits = MathUtils.PopCount(bits);
					if (setBits << 1 > runEnd - bit)
					{
						for (; bit < runEnd; bit++)
						{
							if ((resultBitSet.Bits[bitsIndex] & (1UL << bit)) == 0UL)
							{
								continue;
							}

							var dataIndex = dataOffset + bit;
							action.Apply(bitsOffset + bit,
								ref dataPage1[dataIndex]);
						}
					}
					else
					{
						do
						{
							var dataIndex = dataOffset + bit;
							action.Apply(bitsOffset + bit,
								ref dataPage1[dataIndex]);
							bits &= (bits - 1UL) & resultBitSet.Bits[bitsIndex];
							bit = deBruijn[(int)(((bits & (ulong)-(long)bits) * 0x37E84A99DAE458FUL) >> 58)];
						} while (bits != 0UL);
					}

					block &= (block - 1UL) & resultBitSet.NonEmptyBlocks[blockIndex];
				}
			}

			BitsPool.ReturnAndPop(resultBitSet);
		}

		public static void ForEach<T1, T2, TAction>(this Context context, ref TAction action)
			where TAction : IEntityAction<T1, T2>
		{
			NoDataException.ThrowIfHasNoData<T1>(context.World, DataAccessContext.View);
			NoDataException.ThrowIfHasNoData<T2>(context.World, DataAccessContext.View);

			var dataSet1 = context.World.DataSet<T1>();
			var dataSet2 = context.World.DataSet<T2>();

			var resultBitSet = BitsPool.RentClone(dataSet1)
				.AndBits(dataSet2)
				.RemoveOnRemove(dataSet1)
				.RemoveOnRemove(dataSet2);

			ApplyFilter(context.Filter, resultBitSet);

			var blocksLength = BitSetBase.GetMinBitSet(dataSet1, dataSet2).NonEmptyBlocks.Length;

			var deBruijn = MathUtils.DeBruijn;
			for (var blockIndex = 0; blockIndex < blocksLength; blockIndex++)
			{
				var block = resultBitSet.NonEmptyBlocks[blockIndex];
				var blockOffset = blockIndex << 6;
				while (block != 0UL)
				{
					var blockBit = deBruijn[(int)(((block & (ulong)-(long)block) * 0x37E84A99DAE458FUL) >> 58)];

					var bitsIndex = blockOffset + blockBit;
					var bits = resultBitSet.Bits[bitsIndex];
					var bitsOffset = bitsIndex << 6;
					var bit = deBruijn[(int)(((bits & (ulong)-(long)bits) * 0x37E84A99DAE458FUL) >> 58)];
					var dataOffset = bitsOffset & Constants.PageSizeMinusOne;
					var pageIndex = bitsOffset >> Constants.PageSizePower;
					var dataPage1 = dataSet1.PagedData[pageIndex];
					var dataPage2 = dataSet2.PagedData[pageIndex];

					var runEnd = MathUtils.ApproximateMSB(bits);
					var setBits = MathUtils.PopCount(bits);
					if (setBits << 1 > runEnd - bit)
					{
						for (; bit < runEnd; bit++)
						{
							if ((resultBitSet.Bits[bitsIndex] & (1UL << bit)) == 0UL)
							{
								continue;
							}

							var dataIndex = dataOffset + bit;
							action.Apply(bitsOffset + bit,
								ref dataPage1[dataIndex],
								ref dataPage2[dataIndex]);
						}
					}
					else
					{
						do
						{
							var dataIndex = dataOffset + bit;
							action.Apply(bitsOffset + bit,
								ref dataPage1[dataIndex],
								ref dataPage2[dataIndex]);
							bits &= (bits - 1UL) & resultBitSet.Bits[bitsIndex];
							bit = deBruijn[(int)(((bits & (ulong)-(long)bits) * 0x37E84A99DAE458FUL) >> 58)];
						} while (bits != 0UL);
					}

					block &= (block - 1UL) & resultBitSet.NonEmptyBlocks[blockIndex];
				}
			}

			BitsPool.ReturnAndPop(resultBitSet);
		}

		public static void ForEach<T1, T2, T3, TAction>(this Context context, ref TAction action)
			where TAction : IEntityAction<T1, T2, T3>
		{
			NoDataException.ThrowIfHasNoData<T1>(context.World, DataAccessContext.View);
			NoDataException.ThrowIfHasNoData<T2>(context.World, DataAccessContext.View);
			NoDataException.ThrowIfHasNoData<T3>(context.World, DataAccessContext.View);

			var dataSet1 = context.World.DataSet<T1>();
			var dataSet2 = context.World.DataSet<T2>();
			var dataSet3 = context.World.DataSet<T3>();

			var resultBitSet = BitsPool.RentClone(dataSet1)
				.AndBits(dataSet2)
				.AndBits(dataSet3)
				.RemoveOnRemove(dataSet1)
				.RemoveOnRemove(dataSet2)
				.RemoveOnRemove(dataSet3);

			ApplyFilter(context.Filter, resultBitSet);

			var blocksLength = BitSetBase.GetMinBitSet(dataSet1, dataSet2, dataSet3).NonEmptyBlocks.Length;

			var deBruijn = MathUtils.DeBruijn;
			for (var blockIndex = 0; blockIndex < blocksLength; blockIndex++)
			{
				var block = resultBitSet.NonEmptyBlocks[blockIndex];
				var blockOffset = blockIndex << 6;
				while (block != 0UL)
				{
					var blockBit = deBruijn[(int)(((block & (ulong)-(long)block) * 0x37E84A99DAE458FUL) >> 58)];

					var bitsIndex = blockOffset + blockBit;
					var bits = resultBitSet.Bits[bitsIndex];
					var bitsOffset = bitsIndex << 6;
					var bit = deBruijn[(int)(((bits & (ulong)-(long)bits) * 0x37E84A99DAE458FUL) >> 58)];
					var dataOffset = bitsOffset & Constants.PageSizeMinusOne;
					var pageIndex = bitsOffset >> Constants.PageSizePower;
					var dataPage1 = dataSet1.PagedData[pageIndex];
					var dataPage2 = dataSet2.PagedData[pageIndex];
					var dataPage3 = dataSet3.PagedData[pageIndex];

					var runEnd = MathUtils.ApproximateMSB(bits);
					var setBits = MathUtils.PopCount(bits);
					if (setBits << 1 > runEnd - bit)
					{
						for (; bit < runEnd; bit++)
						{
							if ((resultBitSet.Bits[bitsIndex] & (1UL << bit)) == 0UL)
							{
								continue;
							}

							var dataIndex = dataOffset + bit;
							action.Apply(bitsOffset + bit,
								ref dataPage1[dataIndex],
								ref dataPage2[dataIndex],
								ref dataPage3[dataIndex]);
						}
					}
					else
					{
						do
						{
							var dataIndex = dataOffset + bit;
							action.Apply(bitsOffset + bit,
								ref dataPage1[dataIndex],
								ref dataPage2[dataIndex],
								ref dataPage3[dataIndex]);
							bits &= (bits - 1UL) & resultBitSet.Bits[bitsIndex];
							bit = deBruijn[(int)(((bits & (ulong)-(long)bits) * 0x37E84A99DAE458FUL) >> 58)];
						} while (bits != 0UL);
					}

					block &= (block - 1UL) & resultBitSet.NonEmptyBlocks[blockIndex];
				}
			}

			BitsPool.ReturnAndPop(resultBitSet);
		}

		public static void ForEach<T1, T2, T3, T4, TAction>(this Context context, ref TAction action)
			where TAction : IEntityAction<T1, T2, T3, T4>
		{
			NoDataException.ThrowIfHasNoData<T1>(context.World, DataAccessContext.View);
			NoDataException.ThrowIfHasNoData<T2>(context.World, DataAccessContext.View);
			NoDataException.ThrowIfHasNoData<T3>(context.World, DataAccessContext.View);
			NoDataException.ThrowIfHasNoData<T4>(context.World, DataAccessContext.View);

			var dataSet1 = context.World.DataSet<T1>();
			var dataSet2 = context.World.DataSet<T2>();
			var dataSet3 = context.World.DataSet<T3>();
			var dataSet4 = context.World.DataSet<T4>();

			var resultBitSet = BitsPool.RentClone(dataSet1)
				.AndBits(dataSet2)
				.AndBits(dataSet3)
				.AndBits(dataSet4)
				.RemoveOnRemove(dataSet1)
				.RemoveOnRemove(dataSet2)
				.RemoveOnRemove(dataSet3)
				.RemoveOnRemove(dataSet4);

			ApplyFilter(context.Filter, resultBitSet);

			var blocksLength = BitSetBase.GetMinBitSet(dataSet1, dataSet2, dataSet3, dataSet4).NonEmptyBlocks.Length;

			var deBruijn = MathUtils.DeBruijn;
			for (var blockIndex = 0; blockIndex < blocksLength; blockIndex++)
			{
				var block = resultBitSet.NonEmptyBlocks[blockIndex];
				var blockOffset = blockIndex << 6;
				while (block != 0UL)
				{
					var blockBit = deBruijn[(int)(((block & (ulong)-(long)block) * 0x37E84A99DAE458FUL) >> 58)];

					var bitsIndex = blockOffset + blockBit;
					var bits = resultBitSet.Bits[bitsIndex];
					var bitsOffset = bitsIndex << 6;
					var bit = deBruijn[(int)(((bits & (ulong)-(long)bits) * 0x37E84A99DAE458FUL) >> 58)];
					var dataOffset = bitsOffset & Constants.PageSizeMinusOne;
					var pageIndex = bitsOffset >> Constants.PageSizePower;
					var dataPage1 = dataSet1.PagedData[pageIndex];
					var dataPage2 = dataSet2.PagedData[pageIndex];
					var dataPage3 = dataSet3.PagedData[pageIndex];
					var dataPage4 = dataSet4.PagedData[pageIndex];

					var runEnd = MathUtils.ApproximateMSB(bits);
					var setBits = MathUtils.PopCount(bits);
					if (setBits << 1 > runEnd - bit)
					{
						for (; bit < runEnd; bit++)
						{
							if ((resultBitSet.Bits[bitsIndex] & (1UL << bit)) == 0UL)
							{
								continue;
							}

							var dataIndex = dataOffset + bit;
							action.Apply(bitsOffset + bit,
								ref dataPage1[dataIndex],
								ref dataPage2[dataIndex],
								ref dataPage3[dataIndex],
								ref dataPage4[dataIndex]);
						}
					}
					else
					{
						do
						{
							var dataIndex = dataOffset + bit;
							action.Apply(bitsOffset + bit,
								ref dataPage1[dataIndex],
								ref dataPage2[dataIndex],
								ref dataPage3[dataIndex],
								ref dataPage4[dataIndex]);
							bits &= (bits - 1UL) & resultBitSet.Bits[bitsIndex];
							bit = deBruijn[(int)(((bits & (ulong)-(long)bits) * 0x37E84A99DAE458FUL) >> 58)];
						} while (bits != 0UL);
					}

					block &= (block - 1UL) & resultBitSet.NonEmptyBlocks[blockIndex];
				}
			}

			BitsPool.ReturnAndPop(resultBitSet);
		}

		private static void ApplyFilter(Filter filter, BitSet resultBitSet)
		{
			for (var i = 0; i < filter.IncludedCount; i++)
			{
				var included = filter.Included[i];
				resultBitSet.AndBits(included);
				resultBitSet.RemoveOnRemove(included);
			}

			for (var i = 0; i < filter.ExcludedCount; i++)
			{
				var excluded = filter.Excluded[i];
				resultBitSet.NotBits(excluded);
				resultBitSet.RemoveOnAdd(excluded);
			}
		}
	}
}
