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

			for (var current1 = 0; current1 < bits1Length; current1++)
			{
				var bits = resultBits.Bits1[current1];
				if (bits == 0UL)
				{
					continue;
				}

				var offset = current1 << 6;
				var index = MathUtils.LSB(bits);
				var runEnd = MathUtils.ApproximateMSB(bits);

				var setBitCount = MathUtils.PopCount(bits);
				var runLength = runEnd - index;

				if (setBitCount << 1 > runLength)
				{
					for (; index < runEnd; index++)
					{
						if ((resultBits.Bits1[current1] & (1UL << index)) == 0UL)
						{
							continue;
						}

						Bits0Loop(offset + index, ref action);
					}
					continue;
				}

				Bits0Loop(offset + index, ref action);
				bits &= resultBits.Bits1[current1] & (bits - 1UL);

				while (bits != 0UL)
				{
					index = MathUtils.LSB(bits);
					Bits0Loop(offset + index, ref action);
					bits &= resultBits.Bits1[current1] & (bits - 1UL);
				}
			}

			BitsPool.ReturnAndPop(resultBits);
			return;

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			void Bits0Loop(int current0, ref TAction action)
			{
				var bits = resultBits.Bits0[current0];
				var offset = current0 << 6;
				var index = MathUtils.LSB(bits);
				var runEnd = MathUtils.ApproximateMSB(bits);

				var setBitCount = MathUtils.PopCount(bits);
				var runLength = runEnd - index;

				if (setBitCount << 1 > runLength)
				{
					for (; index < runEnd; index++)
					{
						if ((resultBits.Bits0[current0] & (1UL << index)) == 0UL)
						{
							continue;
						}

						action.Apply(offset + index);
					}
					return;
				}

				action.Apply(offset + index);
				bits &= resultBits.Bits0[current0] & (bits - 1UL);

				while (bits != 0UL)
				{
					index = MathUtils.LSB(bits);
					action.Apply(offset + index);
					bits &= resultBits.Bits0[current0] & (bits - 1UL);
				}
			}
		}

		public void ForEach<TAction, T>(ref TAction action)
			where TAction : IEntityAction<T>
		{
			NoDataException.ThrowIfHasNoData<T>(this, DataAccessContext.View);

			var dataSet = this.DataSet<T>();

			var resultBits = BitsPool.RentClone(dataSet).RemoveOnRemove(dataSet);

			var bits1Length = dataSet.Bits1.Length;

			for (var current1 = 0; current1 < bits1Length; current1++)
			{
				var bits = resultBits.Bits1[current1];
				if (bits == 0UL)
				{
					continue;
				}

				var offset = current1 << 6;
				var index = MathUtils.LSB(bits);
				var runEnd = MathUtils.ApproximateMSB(bits);

				var setBitCount = MathUtils.PopCount(bits);
				var runLength = runEnd - index;

				if (setBitCount << 1 > runLength)
				{
					for (; index < runEnd; index++)
					{
						if ((resultBits.Bits1[current1] & (1UL << index)) == 0UL)
						{
							continue;
						}

						Bits0Loop(offset + index, ref action);
					}
					continue;
				}

				Bits0Loop(offset + index, ref action);
				bits &= resultBits.Bits1[current1] & (bits - 1UL);

				while (bits != 0UL)
				{
					index = MathUtils.LSB(bits);
					Bits0Loop(offset + index, ref action);
					bits &= resultBits.Bits1[current1] & (bits - 1UL);
				}
			}

			BitsPool.ReturnAndPop(resultBits);
			return;

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			void Bits0Loop(int current0, ref TAction action)
			{
				var dataOffset = dataSet.Blocks[current0].StartInPage;
				var dataPage = dataSet.PagedData[dataSet.Blocks[current0].PageIndex];

				var bits = resultBits.Bits0[current0];
				var offset = current0 << 6;
				var index = MathUtils.LSB(bits);
				var runEnd = MathUtils.ApproximateMSB(bits);

				var setBitCount = MathUtils.PopCount(bits);
				var runLength = runEnd - index;

				if (setBitCount << 1 > runLength)
				{
					for (; index < runEnd; index++)
					{
						if ((resultBits.Bits0[current0] & (1UL << index)) == 0UL)
						{
							continue;
						}

						action.Apply(offset + index,
							ref dataPage[dataOffset + index]);
					}
					return;
				}

				action.Apply(offset + index,
					ref dataPage[dataOffset + index]);
				bits &= resultBits.Bits0[current0] & (bits - 1UL);

				while (bits != 0UL)
				{
					index = MathUtils.LSB(bits);
					action.Apply(offset + index,
						ref dataPage[dataOffset + index]);
					bits &= resultBits.Bits0[current0] & (bits - 1UL);
				}
			}
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

			for (var current1 = 0; current1 < bits1Length; current1++)
			{
				var bits = resultBits.Bits1[current1];
				if (bits == 0UL)
				{
					continue;
				}

				var offset = current1 << 6;
				var index = MathUtils.LSB(bits);
				var runEnd = MathUtils.ApproximateMSB(bits);

				var setBitCount = MathUtils.PopCount(bits);
				var runLength = runEnd - index;

				if (setBitCount << 1 > runLength)
				{
					for (; index < runEnd; index++)
					{
						if ((resultBits.Bits1[current1] & (1UL << index)) == 0UL)
						{
							continue;
						}

						Bits0Loop(offset + index, ref action);
					}
					continue;
				}

				Bits0Loop(offset + index, ref action);
				bits &= resultBits.Bits1[current1] & (bits - 1UL);

				while (bits != 0UL)
				{
					index = MathUtils.LSB(bits);
					Bits0Loop(offset + index, ref action);
					bits &= resultBits.Bits1[current1] & (bits - 1UL);
				}
			}

			BitsPool.ReturnAndPop(resultBits);
			return;

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			void Bits0Loop(int current0, ref TAction action)
			{
				var dataOffset1 = dataSet1.Blocks[current0].StartInPage;
				var dataOffset2 = dataSet2.Blocks[current0].StartInPage;
				var dataPage1 = dataSet1.PagedData[dataSet1.Blocks[current0].PageIndex];
				var dataPage2 = dataSet2.PagedData[dataSet2.Blocks[current0].PageIndex];

				var bits = resultBits.Bits0[current0];
				var offset = current0 << 6;
				var index = MathUtils.LSB(bits);
				var runEnd = MathUtils.ApproximateMSB(bits);

				var setBitCount = MathUtils.PopCount(bits);
				var runLength = runEnd - index;

				if (setBitCount << 1 > runLength)
				{
					for (; index < runEnd; index++)
					{
						if ((resultBits.Bits0[current0] & (1UL << index)) == 0UL)
						{
							continue;
						}

						action.Apply(offset + index,
							ref dataPage1[dataOffset1 + index],
							ref dataPage2[dataOffset2 + index]);
					}
					return;
				}

				action.Apply(offset + index,
					ref dataPage1[dataOffset1 + index],
					ref dataPage2[dataOffset2 + index]);
				bits &= resultBits.Bits0[current0] & (bits - 1UL);

				while (bits != 0UL)
				{
					index = MathUtils.LSB(bits);
					action.Apply(offset + index,
						ref dataPage1[dataOffset1 + index],
						ref dataPage2[dataOffset2 + index]);
					bits &= resultBits.Bits0[current0] & (bits - 1UL);
				}
			}
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

			for (var current1 = 0; current1 < bits1Length; current1++)
			{
				var bits = resultBits.Bits1[current1];
				if (bits == 0UL)
				{
					continue;
				}

				var offset = current1 << 6;
				var index = MathUtils.LSB(bits);
				var runEnd = MathUtils.ApproximateMSB(bits);

				var setBitCount = MathUtils.PopCount(bits);
				var runLength = runEnd - index;

				if (setBitCount << 1 > runLength)
				{
					for (; index < runEnd; index++)
					{
						if ((resultBits.Bits1[current1] & (1UL << index)) == 0UL)
						{
							continue;
						}

						Bits0Loop(offset + index, ref action);
					}
					continue;
				}

				Bits0Loop(offset + index, ref action);
				bits &= resultBits.Bits1[current1] & (bits - 1UL);

				while (bits != 0UL)
				{
					index = MathUtils.LSB(bits);
					Bits0Loop(offset + index, ref action);
					bits &= resultBits.Bits1[current1] & (bits - 1UL);
				}
			}

			BitsPool.ReturnAndPop(resultBits);
			return;

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			void Bits0Loop(int current0, ref TAction action)
			{
				var dataOffset1 = dataSet1.Blocks[current0].StartInPage;
				var dataOffset2 = dataSet2.Blocks[current0].StartInPage;
				var dataOffset3 = dataSet3.Blocks[current0].StartInPage;
				var dataPage1 = dataSet1.PagedData[dataSet1.Blocks[current0].PageIndex];
				var dataPage2 = dataSet2.PagedData[dataSet2.Blocks[current0].PageIndex];
				var dataPage3 = dataSet3.PagedData[dataSet3.Blocks[current0].PageIndex];

				var bits = resultBits.Bits0[current0];
				var offset = current0 << 6;
				var index = MathUtils.LSB(bits);
				var runEnd = MathUtils.ApproximateMSB(bits);

				var setBitCount = MathUtils.PopCount(bits);
				var runLength = runEnd - index;

				if (setBitCount << 1 > runLength)
				{
					for (; index < runEnd; index++)
					{
						if ((resultBits.Bits0[current0] & (1UL << index)) == 0UL)
						{
							continue;
						}

						action.Apply(offset + index,
							ref dataPage1[dataOffset1 + index],
							ref dataPage2[dataOffset2 + index],
							ref dataPage3[dataOffset3 + index]);
					}
					return;
				}

				action.Apply(offset + index,
					ref dataPage1[dataOffset1 + index],
					ref dataPage2[dataOffset2 + index],
					ref dataPage3[dataOffset3 + index]);
				bits &= resultBits.Bits0[current0] & (bits - 1UL);

				while (bits != 0UL)
				{
					index = MathUtils.LSB(bits);
					action.Apply(offset + index,
						ref dataPage1[dataOffset1 + index],
						ref dataPage2[dataOffset2 + index],
						ref dataPage3[dataOffset3 + index]);
					bits &= resultBits.Bits0[current0] & (bits - 1UL);
				}
			}
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

			for (var current1 = 0; current1 < bits1Length; current1++)
			{
				var bits = resultBits.Bits1[current1];
				if (bits == 0UL)
				{
					continue;
				}

				var offset = current1 << 6;
				var index = MathUtils.LSB(bits);
				var runEnd = MathUtils.ApproximateMSB(bits);

				var setBitCount = MathUtils.PopCount(bits);
				var runLength = runEnd - index;

				if (setBitCount << 1 > runLength)
				{
					for (; index < runEnd; index++)
					{
						if ((resultBits.Bits1[current1] & (1UL << index)) == 0UL)
						{
							continue;
						}

						Bits0Loop(offset + index, ref action);
					}
					continue;
				}

				Bits0Loop(offset + index, ref action);
				bits &= resultBits.Bits1[current1] & (bits - 1UL);

				while (bits != 0UL)
				{
					index = MathUtils.LSB(bits);
					Bits0Loop(offset + index, ref action);
					bits &= resultBits.Bits1[current1] & (bits - 1UL);
				}
			}

			BitsPool.ReturnAndPop(resultBits);
			return;

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			void Bits0Loop(int current0, ref TAction action)
			{
				var dataOffset1 = dataSet1.Blocks[current0].StartInPage;
				var dataOffset2 = dataSet2.Blocks[current0].StartInPage;
				var dataOffset3 = dataSet3.Blocks[current0].StartInPage;
				var dataOffset4 = dataSet4.Blocks[current0].StartInPage;
				var dataPage1 = dataSet1.PagedData[dataSet1.Blocks[current0].PageIndex];
				var dataPage2 = dataSet2.PagedData[dataSet2.Blocks[current0].PageIndex];
				var dataPage3 = dataSet3.PagedData[dataSet3.Blocks[current0].PageIndex];
				var dataPage4 = dataSet4.PagedData[dataSet4.Blocks[current0].PageIndex];

				var bits = resultBits.Bits0[current0];
				var offset = current0 << 6;
				var index = MathUtils.LSB(bits);
				var runEnd = MathUtils.ApproximateMSB(bits);

				var setBitCount = MathUtils.PopCount(bits);
				var runLength = runEnd - index;

				if (setBitCount << 1 > runLength)
				{
					for (; index < runEnd; index++)
					{
						if ((resultBits.Bits0[current0] & (1UL << index)) == 0UL)
						{
							continue;
						}

						action.Apply(offset + index,
							ref dataPage1[dataOffset1 + index],
							ref dataPage2[dataOffset2 + index],
							ref dataPage3[dataOffset3 + index],
							ref dataPage4[dataOffset4 + index]);
					}
					return;
				}

				action.Apply(offset + index,
					ref dataPage1[dataOffset1 + index],
					ref dataPage2[dataOffset2 + index],
					ref dataPage3[dataOffset3 + index],
					ref dataPage4[dataOffset4 + index]);
				bits &= resultBits.Bits0[current0] & (bits - 1UL);

				while (bits != 0UL)
				{
					index = MathUtils.LSB(bits);
					action.Apply(offset + index,
						ref dataPage1[dataOffset1 + index],
						ref dataPage2[dataOffset2 + index],
						ref dataPage3[dataOffset3 + index],
						ref dataPage4[dataOffset4 + index]);
					bits &= resultBits.Bits0[current0] & (bits - 1UL);
				}
			}
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
