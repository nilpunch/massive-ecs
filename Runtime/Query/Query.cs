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
			var resultBits = BitsPool.Rent();

			int bits1Length;

			if (context.Filter.IncludedCount == 0)
			{
				context.World.Entifiers.CopyBitsTo(resultBits);
				resultBits.RemoveOnRemove(context.World.Entifiers);
				bits1Length = context.World.Entifiers.Bits1.Length;
			}
			else
			{
				var minBits = BitsBase.GetMinBits(context.Filter.Included, context.Filter.IncludedCount);
				minBits.CopyBitsTo(resultBits);
				bits1Length = minBits.Bits1.Length;
			}

			ApplyFilter(context.Filter, resultBits);

			var deBruijn = MathUtils.DeBruijn;
			for (var current1 = 0; current1 < bits1Length; current1++)
			{
				var bits1 = resultBits.Bits1[current1];
				var offset1 = current1 << 6;
				while (bits1 != 0UL)
				{
					var index1 = deBruijn[(int)(((bits1 & (ulong)-(long)bits1) * 0x37E84A99DAE458FUL) >> 58)];

					var current0 = offset1 + index1;
					var bits0 = resultBits.Bits0[current0];
					var offset0 = current0 << 6;
					var index0 = deBruijn[(int)(((bits0 & (ulong)-(long)bits0) * 0x37E84A99DAE458FUL) >> 58)];

					var runEnd = MathUtils.ApproximateMSB(bits0);
					var setBits = MathUtils.PopCount(bits0);
					if (setBits << 1 > runEnd - index0)
					{
						for (; index0 < runEnd; index0++)
						{
							if ((resultBits.Bits0[current0] & (1UL << index0)) == 0UL)
							{
								continue;
							}

							action.Apply(offset0 + index0);
						}
					}
					else
					{
						do
						{
							action.Apply(offset0 + index0);
							bits0 &= (bits0 - 1UL) & resultBits.Bits0[current0];
							index0 = deBruijn[(int)(((bits0 & (ulong)-(long)bits0) * 0x37E84A99DAE458FUL) >> 58)];
						} while (bits0 != 0UL);
					}

					bits1 &= (bits1 - 1UL) & resultBits.Bits1[current1];
				}
			}

			BitsPool.ReturnAndPop(resultBits);
		}

		public static void ForEach<T, TAction>(this Context context, ref TAction action)
			where TAction : IEntityAction<T>
		{
			NoDataException.ThrowIfHasNoData<T>(context.World, DataAccessContext.View);

			var dataSet1 = context.World.DataSet<T>();

			var resultBits = BitsPool.RentClone(dataSet1).RemoveOnRemove(dataSet1);

			ApplyFilter(context.Filter, resultBits);

			var bits1Length = dataSet1.Bits1.Length;

			var deBruijn = MathUtils.DeBruijn;
			for (var current1 = 0; current1 < bits1Length; current1++)
			{
				var bits1 = resultBits.Bits1[current1];
				var offset1 = current1 << 6;
				while (bits1 != 0UL)
				{
					var index1 = deBruijn[(int)(((bits1 & (ulong)-(long)bits1) * 0x37E84A99DAE458FUL) >> 58)];

					var current0 = offset1 + index1;
					var bits0 = resultBits.Bits0[current0];
					var offset0 = current0 << 6;
					var index0 = deBruijn[(int)(((bits0 & (ulong)-(long)bits0) * 0x37E84A99DAE458FUL) >> 58)];
					var dataOffset = offset0 & Constants.PageSizeMinusOne;
					var pageIndex = offset0 >> Constants.PageSizePower;
					var dataPage1 = dataSet1.PagedData[pageIndex];

					var runEnd = MathUtils.ApproximateMSB(bits0);
					var setBits = MathUtils.PopCount(bits0);
					if (setBits << 1 > runEnd - index0)
					{
						for (; index0 < runEnd; index0++)
						{
							if ((resultBits.Bits0[current0] & (1UL << index0)) == 0UL)
							{
								continue;
							}

							var dataIndex = dataOffset + index0;
							action.Apply(offset0 + index0,
								ref dataPage1[dataIndex]);
						}
					}
					else
					{
						do
						{
							var dataIndex = dataOffset + index0;
							action.Apply(offset0 + index0,
								ref dataPage1[dataIndex]);
							bits0 &= (bits0 - 1UL) & resultBits.Bits0[current0];
							index0 = deBruijn[(int)(((bits0 & (ulong)-(long)bits0) * 0x37E84A99DAE458FUL) >> 58)];
						} while (bits0 != 0UL);
					}

					bits1 &= (bits1 - 1UL) & resultBits.Bits1[current1];
				}
			}

			BitsPool.ReturnAndPop(resultBits);
		}

		public static void ForEach<T1, T2, TAction>(this Context context, ref TAction action)
			where TAction : IEntityAction<T1, T2>
		{
			NoDataException.ThrowIfHasNoData<T1>(context.World, DataAccessContext.View);
			NoDataException.ThrowIfHasNoData<T2>(context.World, DataAccessContext.View);

			var dataSet1 = context.World.DataSet<T1>();
			var dataSet2 = context.World.DataSet<T2>();

			var resultBits = BitsPool.RentClone(dataSet1)
				.AndBits(dataSet2)
				.RemoveOnRemove(dataSet1)
				.RemoveOnRemove(dataSet2);

			ApplyFilter(context.Filter, resultBits);

			var bits1Length = BitsBase.GetMinBits(dataSet1, dataSet2).Bits1.Length;

			var deBruijn = MathUtils.DeBruijn;
			for (var current1 = 0; current1 < bits1Length; current1++)
			{
				var bits1 = resultBits.Bits1[current1];
				var offset1 = current1 << 6;
				while (bits1 != 0UL)
				{
					var index1 = deBruijn[(int)(((bits1 & (ulong)-(long)bits1) * 0x37E84A99DAE458FUL) >> 58)];

					var current0 = offset1 + index1;
					var bits0 = resultBits.Bits0[current0];
					var offset0 = current0 << 6;
					var index0 = deBruijn[(int)(((bits0 & (ulong)-(long)bits0) * 0x37E84A99DAE458FUL) >> 58)];
					var dataOffset = offset0 & Constants.PageSizeMinusOne;
					var pageIndex = offset0 >> Constants.PageSizePower;
					var dataPage1 = dataSet1.PagedData[pageIndex];
					var dataPage2 = dataSet2.PagedData[pageIndex];

					var runEnd = MathUtils.ApproximateMSB(bits0);
					var setBits = MathUtils.PopCount(bits0);
					if (setBits << 1 > runEnd - index0)
					{
						for (; index0 < runEnd; index0++)
						{
							if ((resultBits.Bits0[current0] & (1UL << index0)) == 0UL)
							{
								continue;
							}

							var dataIndex = dataOffset + index0;
							action.Apply(offset0 + index0,
								ref dataPage1[dataIndex],
								ref dataPage2[dataIndex]);
						}
					}
					else
					{
						do
						{
							var dataIndex = dataOffset + index0;
							action.Apply(offset0 + index0,
								ref dataPage1[dataIndex],
								ref dataPage2[dataIndex]);
							bits0 &= (bits0 - 1UL) & resultBits.Bits0[current0];
							index0 = deBruijn[(int)(((bits0 & (ulong)-(long)bits0) * 0x37E84A99DAE458FUL) >> 58)];
						} while (bits0 != 0UL);
					}

					bits1 &= (bits1 - 1UL) & resultBits.Bits1[current1];
				}
			}

			BitsPool.ReturnAndPop(resultBits);
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

			var resultBits = BitsPool.RentClone(dataSet1)
				.AndBits(dataSet2)
				.AndBits(dataSet3)
				.RemoveOnRemove(dataSet1)
				.RemoveOnRemove(dataSet2)
				.RemoveOnRemove(dataSet3);

			ApplyFilter(context.Filter, resultBits);

			var bits1Length = BitsBase.GetMinBits(dataSet1, dataSet2, dataSet3).Bits1.Length;

			var deBruijn = MathUtils.DeBruijn;
			for (var current1 = 0; current1 < bits1Length; current1++)
			{
				var bits1 = resultBits.Bits1[current1];
				var offset1 = current1 << 6;
				while (bits1 != 0UL)
				{
					var index1 = deBruijn[(int)(((bits1 & (ulong)-(long)bits1) * 0x37E84A99DAE458FUL) >> 58)];

					var current0 = offset1 + index1;
					var bits0 = resultBits.Bits0[current0];
					var offset0 = current0 << 6;
					var index0 = deBruijn[(int)(((bits0 & (ulong)-(long)bits0) * 0x37E84A99DAE458FUL) >> 58)];
					var dataOffset = offset0 & Constants.PageSizeMinusOne;
					var pageIndex = offset0 >> Constants.PageSizePower;
					var dataPage1 = dataSet1.PagedData[pageIndex];
					var dataPage2 = dataSet2.PagedData[pageIndex];
					var dataPage3 = dataSet3.PagedData[pageIndex];

					var runEnd = MathUtils.ApproximateMSB(bits0);
					var setBits = MathUtils.PopCount(bits0);
					if (setBits << 1 > runEnd - index0)
					{
						for (; index0 < runEnd; index0++)
						{
							if ((resultBits.Bits0[current0] & (1UL << index0)) == 0UL)
							{
								continue;
							}

							var dataIndex = dataOffset + index0;
							action.Apply(offset0 + index0,
								ref dataPage1[dataIndex],
								ref dataPage2[dataIndex],
								ref dataPage3[dataIndex]);
						}
					}
					else
					{
						do
						{
							var dataIndex = dataOffset + index0;
							action.Apply(offset0 + index0,
								ref dataPage1[dataIndex],
								ref dataPage2[dataIndex],
								ref dataPage3[dataIndex]);
							bits0 &= (bits0 - 1UL) & resultBits.Bits0[current0];
							index0 = deBruijn[(int)(((bits0 & (ulong)-(long)bits0) * 0x37E84A99DAE458FUL) >> 58)];
						} while (bits0 != 0UL);
					}

					bits1 &= (bits1 - 1UL) & resultBits.Bits1[current1];
				}
			}

			BitsPool.ReturnAndPop(resultBits);
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

			var resultBits = BitsPool.RentClone(dataSet1)
				.AndBits(dataSet2)
				.AndBits(dataSet3)
				.AndBits(dataSet4)
				.RemoveOnRemove(dataSet1)
				.RemoveOnRemove(dataSet2)
				.RemoveOnRemove(dataSet3)
				.RemoveOnRemove(dataSet4);

			ApplyFilter(context.Filter, resultBits);

			var bits1Length = BitsBase.GetMinBits(dataSet1, dataSet2, dataSet3, dataSet4).Bits1.Length;

			var deBruijn = MathUtils.DeBruijn;
			for (var current1 = 0; current1 < bits1Length; current1++)
			{
				var bits1 = resultBits.Bits1[current1];
				var offset1 = current1 << 6;
				while (bits1 != 0UL)
				{
					var index1 = deBruijn[(int)(((bits1 & (ulong)-(long)bits1) * 0x37E84A99DAE458FUL) >> 58)];

					var current0 = offset1 + index1;
					var bits0 = resultBits.Bits0[current0];
					var offset0 = current0 << 6;
					var index0 = deBruijn[(int)(((bits0 & (ulong)-(long)bits0) * 0x37E84A99DAE458FUL) >> 58)];
					var dataOffset = offset0 & Constants.PageSizeMinusOne;
					var pageIndex = offset0 >> Constants.PageSizePower;
					var dataPage1 = dataSet1.PagedData[pageIndex];
					var dataPage2 = dataSet2.PagedData[pageIndex];
					var dataPage3 = dataSet3.PagedData[pageIndex];
					var dataPage4 = dataSet4.PagedData[pageIndex];

					var runEnd = MathUtils.ApproximateMSB(bits0);
					var setBits = MathUtils.PopCount(bits0);
					if (setBits << 1 > runEnd - index0)
					{
						for (; index0 < runEnd; index0++)
						{
							if ((resultBits.Bits0[current0] & (1UL << index0)) == 0UL)
							{
								continue;
							}

							var dataIndex = dataOffset + index0;
							action.Apply(offset0 + index0,
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
							var dataIndex = dataOffset + index0;
							action.Apply(offset0 + index0,
								ref dataPage1[dataIndex],
								ref dataPage2[dataIndex],
								ref dataPage3[dataIndex],
								ref dataPage4[dataIndex]);
							bits0 &= (bits0 - 1UL) & resultBits.Bits0[current0];
							index0 = deBruijn[(int)(((bits0 & (ulong)-(long)bits0) * 0x37E84A99DAE458FUL) >> 58)];
						} while (bits0 != 0UL);
					}

					bits1 &= (bits1 - 1UL) & resultBits.Bits1[current1];
				}
			}

			BitsPool.ReturnAndPop(resultBits);
		}

		private static void ApplyFilter(Filter filter, Bits resultBits)
		{
			for (var i = 0; i < filter.IncludedCount; i++)
			{
				var included = filter.Included[i];
				resultBits.AndBits(included);
				resultBits.RemoveOnRemove(included);
			}

			for (var i = 0; i < filter.ExcludedCount; i++)
			{
				var excluded = filter.Excluded[i];
				resultBits.NotBits(excluded);
				resultBits.RemoveOnAdd(excluded);
			}
		}
	}
}
