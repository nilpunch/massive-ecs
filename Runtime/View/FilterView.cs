#if !MASSIVE_DISABLE_ASSERT
#define MASSIVE_ASSERT
#endif

using System.Runtime.CompilerServices;
using Unity.IL2CPP.CompilerServices;

namespace Massive
{
	[Il2CppSetOption(Option.NullChecks, false)]
	[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
	public readonly struct FilterView : IViewT, IViewTT, IViewTTT, IViewTTTT
	{
		public World World { get; }
		public Filter Filter { get; }

		public FilterView(World world, Filter filter = null)
		{
			World = world;
			Filter = filter ?? world.Filters.Empty;
		}

		public void ForEach<TAction>(ref TAction action)
			where TAction : IEntityAction
		{
			var resultBits = BitsPool.Rent();

			int bits1Length;

			if (Filter.IncludedCount == 0)
			{
				World.Entifiers.CopyBitsTo(resultBits);
				resultBits.RemoveOnRemove(World.Entifiers);
				bits1Length = World.Entifiers.Bits1.Length;
			}
			else
			{
				var minBits = BitsBase.GetMinBits(Filter.Included, Filter.IncludedCount);
				minBits.CopyBitsTo(resultBits);
				bits1Length = minBits.Bits1.Length;
			}

			ApplyFilter(resultBits);

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
					while (bits0 != 0UL)
					{
						var index0 = deBruijn[(int)(((bits0 & (ulong)-(long)bits0) * 0x37E84A99DAE458FUL) >> 58)];
						action.Apply(offset0 + index0);
						bits0 &= (bits0 - 1UL) & resultBits.Bits0[current0];
					}

					bits1 &= (bits1 - 1UL) & resultBits.Bits1[current1];
				}
			}

			BitsPool.ReturnAndPop(resultBits);
		}

		public void ForEach<TAction, T>(ref TAction action)
			where TAction : IEntityAction<T>
		{
			NoDataException.ThrowIfHasNoData<T>(World, DataAccessContext.View);

			var dataSet1 = World.DataSet<T>();

			var resultBits = BitsPool.RentClone(dataSet1).RemoveOnRemove(dataSet1);

			ApplyFilter(resultBits);

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
					var dataOffset = offset0 & Constants.PageSizeMinusOne;
					var dataPage = dataSet1.PagedData[offset0 >> Constants.PageSizePower];
					while (bits0 != 0UL)
					{
						var index0 = deBruijn[(int)(((bits0 & (ulong)-(long)bits0) * 0x37E84A99DAE458FUL) >> 58)];
						action.Apply(offset0 + index0, ref dataPage[dataOffset + index0]);
						bits0 &= (bits0 - 1UL) & resultBits.Bits0[current0];
					}

					bits1 &= (bits1 - 1UL) & resultBits.Bits1[current1];
				}
			}

			BitsPool.ReturnAndPop(resultBits);
		}

		public void ForEach<TAction, T1, T2>(ref TAction action)
			where TAction : IEntityAction<T1, T2>
		{
			NoDataException.ThrowIfHasNoData<T1>(World, DataAccessContext.View);
			NoDataException.ThrowIfHasNoData<T2>(World, DataAccessContext.View);

			var dataSet1 = World.DataSet<T1>();
			var dataSet2 = World.DataSet<T2>();

			var resultBits = BitsPool.RentClone(dataSet1)
				.AndBits(dataSet2)
				.RemoveOnRemove(dataSet1)
				.RemoveOnRemove(dataSet2);

			ApplyFilter(resultBits);

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
					var dataOffset = offset0 & Constants.PageSizeMinusOne;
					var pageIndex = offset0 >> Constants.PageSizePower;
					var dataPage1 = dataSet1.PagedData[pageIndex];
					var dataPage2 = dataSet2.PagedData[pageIndex];
					while (bits0 != 0UL)
					{
						var index0 = deBruijn[(int)(((bits0 & (ulong)-(long)bits0) * 0x37E84A99DAE458FUL) >> 58)];
						var dataIndex = dataOffset + index0;
						action.Apply(offset0 + index0,
							ref dataPage1[dataIndex],
							ref dataPage2[dataIndex]);
						bits0 &= (bits0 - 1UL) & resultBits.Bits0[current0];
					}

					bits1 &= (bits1 - 1UL) & resultBits.Bits1[current1];
				}
			}

			BitsPool.ReturnAndPop(resultBits);
		}

		public void ForEach<TAction, T1, T2, T3>(ref TAction action)
			where TAction : IEntityAction<T1, T2, T3>
		{
			NoDataException.ThrowIfHasNoData<T1>(World, DataAccessContext.View);
			NoDataException.ThrowIfHasNoData<T2>(World, DataAccessContext.View);
			NoDataException.ThrowIfHasNoData<T3>(World, DataAccessContext.View);

			var dataSet1 = World.DataSet<T1>();
			var dataSet2 = World.DataSet<T2>();
			var dataSet3 = World.DataSet<T3>();

			var resultBits = BitsPool.RentClone(dataSet1)
				.AndBits(dataSet2)
				.AndBits(dataSet3)
				.RemoveOnRemove(dataSet1)
				.RemoveOnRemove(dataSet2)
				.RemoveOnRemove(dataSet3);

			ApplyFilter(resultBits);

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
					var dataOffset = offset0 & Constants.PageSizeMinusOne;
					var pageIndex = offset0 >> Constants.PageSizePower;
					var dataPage1 = dataSet1.PagedData[pageIndex];
					var dataPage2 = dataSet2.PagedData[pageIndex];
					var dataPage3 = dataSet3.PagedData[pageIndex];
					while (bits0 != 0UL)
					{
						var index0 = deBruijn[(int)(((bits0 & (ulong)-(long)bits0) * 0x37E84A99DAE458FUL) >> 58)];
						var dataIndex = dataOffset + index0;
						action.Apply(offset0 + index0,
							ref dataPage1[dataIndex],
							ref dataPage2[dataIndex],
							ref dataPage3[dataIndex]);
						bits0 &= (bits0 - 1UL) & resultBits.Bits0[current0];
					}

					bits1 &= (bits1 - 1UL) & resultBits.Bits1[current1];
				}
			}

			BitsPool.ReturnAndPop(resultBits);
		}

		public void ForEach<TAction, T1, T2, T3, T4>(ref TAction action)
			where TAction : IEntityAction<T1, T2, T3, T4>
		{
			NoDataException.ThrowIfHasNoData<T1>(World, DataAccessContext.View);
			NoDataException.ThrowIfHasNoData<T2>(World, DataAccessContext.View);
			NoDataException.ThrowIfHasNoData<T3>(World, DataAccessContext.View);
			NoDataException.ThrowIfHasNoData<T4>(World, DataAccessContext.View);

			var dataSet1 = World.DataSet<T1>();
			var dataSet2 = World.DataSet<T2>();
			var dataSet3 = World.DataSet<T3>();
			var dataSet4 = World.DataSet<T4>();

			var resultBits = BitsPool.RentClone(dataSet1)
				.AndBits(dataSet2)
				.AndBits(dataSet3)
				.AndBits(dataSet4)
				.RemoveOnRemove(dataSet1)
				.RemoveOnRemove(dataSet2)
				.RemoveOnRemove(dataSet3)
				.RemoveOnRemove(dataSet4);

			ApplyFilter(resultBits);

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
					var dataOffset = offset0 & Constants.PageSizeMinusOne;
					var pageIndex = offset0 >> Constants.PageSizePower;
					var dataPage1 = dataSet1.PagedData[pageIndex];
					var dataPage2 = dataSet2.PagedData[pageIndex];
					var dataPage3 = dataSet3.PagedData[pageIndex];
					var dataPage4 = dataSet4.PagedData[pageIndex];
					while (bits0 != 0UL)
					{
						var index0 = deBruijn[(int)(((bits0 & (ulong)-(long)bits0) * 0x37E84A99DAE458FUL) >> 58)];
						var dataIndex = dataOffset + index0;
						action.Apply(offset0 + index0,
							ref dataPage1[dataIndex],
							ref dataPage2[dataIndex],
							ref dataPage3[dataIndex],
							ref dataPage4[dataIndex]);
						bits0 &= (bits0 - 1UL) & resultBits.Bits0[current0];
					}

					bits1 &= (bits1 - 1UL) & resultBits.Bits1[current1];
				}
			}

			BitsPool.ReturnAndPop(resultBits);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public BitsEnumerator GetEnumerator()
		{
			var resultBits = BitsPool.Rent();

			int bits1Length;

			if (Filter.IncludedCount == 0)
			{
				World.Entifiers.CopyBitsTo(resultBits);
				resultBits.RemoveOnRemove(World.Entifiers);
				bits1Length = World.Entifiers.Bits1.Length;
			}
			else
			{
				var minBits = BitsBase.GetMinBits(Filter.Included, Filter.IncludedCount);
				minBits.CopyBitsTo(resultBits);
				bits1Length = minBits.Bits1.Length;
			}

			ApplyFilter(resultBits);

			return new BitsEnumerator(resultBits, bits1Length);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public EntityEnumerable Entities()
		{
			var resultBits = BitsPool.Rent();

			int bits1Length;

			if (Filter.IncludedCount == 0)
			{
				World.Entifiers.CopyBitsTo(resultBits);
				resultBits.RemoveOnRemove(World.Entifiers);
				bits1Length = World.Entifiers.Bits1.Length;
			}
			else
			{
				var minBits = BitsBase.GetMinBits(Filter.Included, Filter.IncludedCount);
				minBits.CopyBitsTo(resultBits);
				bits1Length = minBits.Bits1.Length;
			}

			ApplyFilter(resultBits);

			return new EntityEnumerable(resultBits, World, bits1Length);
		}

		private void ApplyFilter(Bits resultBits)
		{
			for (var i = 0; i < Filter.IncludedCount; i++)
			{
				var included = Filter.Included[i];
				resultBits.AndBits(included);
				resultBits.RemoveOnRemove(included);
			}

			for (var i = 0; i < Filter.ExcludedCount; i++)
			{
				var excluded = Filter.Excluded[i];
				resultBits.NotBits(excluded);
				resultBits.RemoveOnAdd(excluded);
			}
		}
	}
}
