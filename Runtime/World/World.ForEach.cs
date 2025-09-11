using System.Runtime.CompilerServices;
using Unity.IL2CPP.CompilerServices;

namespace Massive
{
	[Il2CppSetOption(Option.NullChecks, false)]
	[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
	public partial class World : IViewT, IViewTT, IViewTTT, IViewTTTT
	{
		World IView.World => this;

		public void ForEach<TAction>(ref TAction action)
			where TAction : IEntityAction
		{
			var entifiers = Entifiers;

			var resultBits = BitsPool.RentClone(entifiers).RemoveOnRemove(entifiers);

			var bits1Length = entifiers.Bits1.Length;

			var deBruijn = MathUtils.DeBruijn;
			for (var current1 = 0; current1 < bits1Length; current1++)
			{
				var bits1 = resultBits.Bits1[current1];
				var offset1 = current1 << 6;
				while (bits1 != 0UL)
				{
					var index1 = deBruijn[((bits1 & (~bits1 + 1UL)) * 0x37E84A99DAE458FUL) >> 58];

					var current0 = offset1 + index1;
					var bits0 = resultBits.Bits0[current0];
					var offset0 = current0 << 6;
					while (bits0 != 0UL)
					{
						var index0 = deBruijn[((bits0 & (~bits0 + 1UL)) * 0x37E84A99DAE458FUL) >> 58];
						action.Apply(offset0 + index0);
						bits0 &= (bits0 - 1UL);
					}

					bits1 &= (bits1 - 1UL);
				}
			}

			BitsPool.ReturnAndPop(resultBits);
		}

		public void ForEach<TAction, T>(ref TAction action)
			where TAction : IEntityAction<T>
		{
			NoDataException.ThrowIfHasNoData<T>(this, DataAccessContext.View);

			var dataSet1 = this.DataSet<T>();

			var resultBits = BitsPool.RentClone(dataSet1).RemoveOnRemove(dataSet1);

			var bits1Length = dataSet1.Bits1.Length;

			var deBruijn = MathUtils.DeBruijn;
			for (var current1 = 0; current1 < bits1Length; current1++)
			{
				var bits1 = resultBits.Bits1[current1];
				var offset1 = current1 << 6;
				while (bits1 != 0UL)
				{
					var index1 = deBruijn[((bits1 & (~bits1 + 1UL)) * 0x37E84A99DAE458FUL) >> 58];

					var current0 = offset1 + index1;
					var dataOffset1 = dataSet1.Blocks[current0].StartInPage;
					var dataPage1 = dataSet1.PagedData[dataSet1.Blocks[current0].PageIndex];
					var bits0 = resultBits.Bits0[current0];
					var offset0 = current0 << 6;
					while (bits0 != 0UL)
					{
						var index0 = deBruijn[((bits0 & (~bits0 + 1UL)) * 0x37E84A99DAE458FUL) >> 58];
						action.Apply(offset0 + index0,
							ref dataPage1[dataOffset1 + index0]);
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
			NoDataException.ThrowIfHasNoData<T1>(this, DataAccessContext.View);
			NoDataException.ThrowIfHasNoData<T2>(this, DataAccessContext.View);

			var dataSet1 = this.DataSet<T1>();
			var dataSet2 = this.DataSet<T2>();

			var resultBits = BitsPool.RentClone(dataSet1)
				.AndBits(dataSet2)
				.RemoveOnRemove(dataSet1)
				.RemoveOnRemove(dataSet2);

			var bits1Length = BitsBase.GetMinBits(dataSet1, dataSet2).Bits1.Length;

			var deBruijn = MathUtils.DeBruijn;
			for (var current1 = 0; current1 < bits1Length; current1++)
			{
				var bits1 = resultBits.Bits1[current1];
				var offset1 = current1 << 6;
				while (bits1 != 0UL)
				{
					var index1 = deBruijn[((bits1 & (~bits1 + 1UL)) * 0x37E84A99DAE458FUL) >> 58];

					var current0 = offset1 + index1;
					var dataOffset1 = dataSet1.Blocks[current0].StartInPage;
					var dataOffset2 = dataSet2.Blocks[current0].StartInPage;
					var dataPage1 = dataSet1.PagedData[dataSet1.Blocks[current0].PageIndex];
					var dataPage2 = dataSet2.PagedData[dataSet2.Blocks[current0].PageIndex];
					var bits0 = resultBits.Bits0[current0];
					var offset0 = current0 << 6;
					while (bits0 != 0UL)
					{
						var index0 = deBruijn[((bits0 & (~bits0 + 1UL)) * 0x37E84A99DAE458FUL) >> 58];
						action.Apply(offset0 + index0,
							ref dataPage1[dataOffset1 + index0],
							ref dataPage2[dataOffset2 + index0]);
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
			NoDataException.ThrowIfHasNoData<T1>(this, DataAccessContext.View);
			NoDataException.ThrowIfHasNoData<T2>(this, DataAccessContext.View);
			NoDataException.ThrowIfHasNoData<T3>(this, DataAccessContext.View);

			var dataSet1 = this.DataSet<T1>();
			var dataSet2 = this.DataSet<T2>();
			var dataSet3 = this.DataSet<T3>();

			var resultBits = BitsPool.RentClone(dataSet1)
				.AndBits(dataSet2)
				.AndBits(dataSet3)
				.RemoveOnRemove(dataSet1)
				.RemoveOnRemove(dataSet2)
				.RemoveOnRemove(dataSet3);

			var bits1Length = BitsBase.GetMinBits(dataSet1, dataSet2, dataSet3).Bits1.Length;

			var deBruijn = MathUtils.DeBruijn;
			for (var current1 = 0; current1 < bits1Length; current1++)
			{
				var bits1 = resultBits.Bits1[current1];
				var offset1 = current1 << 6;
				while (bits1 != 0UL)
				{
					var index1 = deBruijn[((bits1 & (~bits1 + 1UL)) * 0x37E84A99DAE458FUL) >> 58];

					var current0 = offset1 + index1;
					var dataOffset1 = dataSet1.Blocks[current0].StartInPage;
					var dataOffset2 = dataSet2.Blocks[current0].StartInPage;
					var dataOffset3 = dataSet3.Blocks[current0].StartInPage;
					var dataPage1 = dataSet1.PagedData[dataSet1.Blocks[current0].PageIndex];
					var dataPage2 = dataSet2.PagedData[dataSet2.Blocks[current0].PageIndex];
					var dataPage3 = dataSet3.PagedData[dataSet3.Blocks[current0].PageIndex];
					var bits0 = resultBits.Bits0[current0];
					var offset0 = current0 << 6;
					while (bits0 != 0UL)
					{
						var index0 = deBruijn[((bits0 & (~bits0 + 1UL)) * 0x37E84A99DAE458FUL) >> 58];
						action.Apply(offset0 + index0,
							ref dataPage1[dataOffset1 + index0],
							ref dataPage2[dataOffset2 + index0],
							ref dataPage3[dataOffset3 + index0]);
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
			NoDataException.ThrowIfHasNoData<T1>(this, DataAccessContext.View);
			NoDataException.ThrowIfHasNoData<T2>(this, DataAccessContext.View);
			NoDataException.ThrowIfHasNoData<T3>(this, DataAccessContext.View);
			NoDataException.ThrowIfHasNoData<T4>(this, DataAccessContext.View);

			var dataSet1 = this.DataSet<T1>();
			var dataSet2 = this.DataSet<T2>();
			var dataSet3 = this.DataSet<T3>();
			var dataSet4 = this.DataSet<T4>();

			var resultBits = BitsPool.RentClone(dataSet1)
				.AndBits(dataSet2)
				.AndBits(dataSet3)
				.AndBits(dataSet4)
				.RemoveOnRemove(dataSet1)
				.RemoveOnRemove(dataSet2)
				.RemoveOnRemove(dataSet3)
				.RemoveOnRemove(dataSet4);

			var bits1Length = BitsBase.GetMinBits(dataSet1, dataSet2, dataSet3, dataSet4).Bits1.Length;

			var deBruijn = MathUtils.DeBruijn;
			for (var current1 = 0; current1 < bits1Length; current1++)
			{
				var bits1 = resultBits.Bits1[current1];
				var offset1 = current1 << 6;
				while (bits1 != 0UL)
				{
					var index1 = deBruijn[((bits1 & (~bits1 + 1UL)) * 0x37E84A99DAE458FUL) >> 58];

					var current0 = offset1 + index1;
					var dataOffset1 = dataSet1.Blocks[current0].StartInPage;
					var dataOffset2 = dataSet2.Blocks[current0].StartInPage;
					var dataOffset3 = dataSet3.Blocks[current0].StartInPage;
					var dataOffset4 = dataSet4.Blocks[current0].StartInPage;
					var dataPage1 = dataSet1.PagedData[dataSet1.Blocks[current0].PageIndex];
					var dataPage2 = dataSet2.PagedData[dataSet2.Blocks[current0].PageIndex];
					var dataPage3 = dataSet3.PagedData[dataSet3.Blocks[current0].PageIndex];
					var dataPage4 = dataSet4.PagedData[dataSet4.Blocks[current0].PageIndex];
					var bits0 = resultBits.Bits0[current0];
					var offset0 = current0 << 6;
					while (bits0 != 0UL)
					{
						var index0 = deBruijn[((bits0 & (~bits0 + 1UL)) * 0x37E84A99DAE458FUL) >> 58];
						action.Apply(offset0 + index0,
							ref dataPage1[dataOffset1 + index0],
							ref dataPage2[dataOffset2 + index0],
							ref dataPage3[dataOffset3 + index0],
							ref dataPage4[dataOffset4 + index0]);
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
			var bits = BitsPool.RentClone(Entifiers).RemoveOnRemove(Entifiers);
			return new BitsEnumerator(bits, Entifiers.Bits1.Length);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public EntityEnumerable Entities()
		{
			var bits = BitsPool.RentClone(Entifiers).RemoveOnRemove(Entifiers);
			return new EntityEnumerable(bits, this, Entifiers.Bits1.Length);
		}
	}
}
