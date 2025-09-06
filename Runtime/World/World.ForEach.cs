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

			var resultBits = BitsPool.RentClone(entifiers);

			entifiers.PushRemoveOnRemove(resultBits);

			var bits1Length = entifiers.Bits1.Length;

			for (var current1 = 0; current1 < bits1Length; current1++)
			{
				var bits1 = resultBits.Bits1[current1];
				if (bits1 == 0UL)
				{
					continue;
				}

				var offset1 = current1 << 6;
				var iterated1 = MathUtils.TZC(bits1);

				if (MathUtils.HasLessThen3Runs(bits1))
				{
					bits1 >>= iterated1;
					var runEnd1 = bits1 == ulong.MaxValue ? 64 : iterated1 + MathUtils.TZC(~bits1);
					for (; iterated1 < runEnd1; iterated1++)
					{
						if ((resultBits.Bits1[current1] & (1UL << iterated1)) == 0UL)
						{
							continue;
						}

						Bits0Loop(offset1, iterated1, ref action);
					}
					continue;
				}

				Bits0Loop(offset1, iterated1, ref action);
				resultBits.Bits1[current1] &= bits1 - 1UL;

				while (resultBits.Bits1[current1] != 0UL)
				{
					bits1 = resultBits.Bits1[current1];
					iterated1 = MathUtils.TZC(bits1);
					Bits0Loop(offset1, iterated1, ref action);
					resultBits.Bits1[current1] &= bits1 - 1UL;
				}
			}

			entifiers.PopRemoveOnRemove();

			BitsPool.Return(resultBits);
			return;

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			void Bits0Loop(int offset1, int iterated1, ref TAction action)
			{
				var current0 = offset1 + iterated1;
				var bits0 = resultBits.Bits0[current0];
				if (bits0 == 0UL)
				{
					return;
				}

				var offset0 = current0 << 6;
				var iterated0 = MathUtils.TZC(bits0);

				if (MathUtils.HasLessThen3Runs(bits0))
				{
					bits0 >>= iterated0;
					var runEnd0 = bits0 == ulong.MaxValue ? 64 : iterated0 + MathUtils.TZC(~bits0);
					for (; iterated0 < runEnd0; iterated0++)
					{
						if ((resultBits.Bits0[current0] & (1UL << iterated0)) == 0UL)
						{
							continue;
						}

						action.Apply(offset0 + iterated0);
					}
					return;
				}

				action.Apply(offset0 + iterated0);
				resultBits.Bits0[current0] &= bits0 - 1UL;

				while (resultBits.Bits0[current0] != 0UL)
				{
					bits0 = resultBits.Bits0[current0];
					iterated0 = MathUtils.TZC(bits0);
					action.Apply(offset0 + iterated0);
					resultBits.Bits0[current0] &= bits0 - 1UL;
				}
			}
		}

		public void ForEach<TAction, T>(ref TAction action)
			where TAction : IEntityAction<T>
		{
			NoDataException.ThrowIfHasNoData<T>(this, DataAccessContext.View);

			var dataSet = this.DataSet<T>();

			var resultBits = BitsPool.RentClone(dataSet);

			dataSet.PushRemoveOnRemove(resultBits);

			var bits1Length = dataSet.Bits1.Length;

			for (var current1 = 0; current1 < bits1Length; current1++)
			{
				var bits1 = resultBits.Bits1[current1];
				if (bits1 == 0UL)
				{
					continue;
				}

				var offset1 = current1 << 6;
				var iterated1 = MathUtils.TZC(bits1);

				if (MathUtils.HasLessThen3Runs(bits1))
				{
					bits1 >>= iterated1;
					var runEnd1 = bits1 == ulong.MaxValue ? 64 : iterated1 + MathUtils.TZC(~bits1);
					for (; iterated1 < runEnd1; iterated1++)
					{
						if ((resultBits.Bits1[current1] & (1UL << iterated1)) == 0UL)
						{
							continue;
						}

						Bits0Loop(offset1, iterated1, ref action);
					}
					continue;
				}

				Bits0Loop(offset1, iterated1, ref action);
				resultBits.Bits1[current1] &= bits1 - 1UL;

				while (resultBits.Bits1[current1] != 0UL)
				{
					bits1 = resultBits.Bits1[current1];
					iterated1 = MathUtils.TZC(bits1);
					Bits0Loop(offset1, iterated1, ref action);
					resultBits.Bits1[current1] &= bits1 - 1UL;
				}
			}

			dataSet.PopRemoveOnRemove();

			BitsPool.Return(resultBits);
			return;

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			void Bits0Loop(int offset1, int iterated1, ref TAction action)
			{
				var current0 = offset1 + iterated1;
				var bits0 = resultBits.Bits0[current0];
				if (bits0 == 0UL)
				{
					return;
				}

				var dataOffset = dataSet.Pages[current0].DataIndex & dataSet.PageSizeMinusOne;
				var dataPage = dataSet.Data[dataSet.Pages[current0].DataIndex >> dataSet.PageSizePower];

				var offset0 = current0 << 6;
				var iterated0 = MathUtils.TZC(bits0);

				if (MathUtils.HasLessThen3Runs(bits0))
				{
					bits0 >>= iterated0;
					var runEnd0 = bits0 == ulong.MaxValue ? 64 : iterated0 + MathUtils.TZC(~bits0);
					for (; iterated0 < runEnd0; iterated0++)
					{
						if ((resultBits.Bits0[current0] & (1UL << iterated0)) == 0UL)
						{
							continue;
						}

						action.Apply(offset0 + iterated0,
							ref dataPage[dataOffset + iterated0]);
					}
					return;
				}

				action.Apply(offset0 + iterated0,
					ref dataPage[dataOffset + iterated0]);
				resultBits.Bits0[current0] &= bits0 - 1UL;

				while (resultBits.Bits0[current0] != 0UL)
				{
					bits0 = resultBits.Bits0[current0];
					iterated0 = MathUtils.TZC(bits0);
					action.Apply(offset0 + iterated0,
						ref dataPage[dataOffset + iterated0]);
					resultBits.Bits0[current0] &= bits0 - 1UL;
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

			var resultBits = BitsPool.RentClone(dataSet1).And(dataSet2);

			dataSet1.PushRemoveOnRemove(resultBits);
			dataSet2.PushRemoveOnRemove(resultBits);

			var bits1Length = BitsBase.GetMinBits(dataSet1, dataSet2).Bits1.Length;

			for (var current1 = 0; current1 < bits1Length; current1++)
			{
				var bits1 = resultBits.Bits1[current1];
				if (bits1 == 0UL)
				{
					continue;
				}

				var offset1 = current1 << 6;
				var iterated1 = MathUtils.TZC(bits1);

				if (MathUtils.HasLessThen3Runs(bits1))
				{
					bits1 >>= iterated1;
					var runEnd1 = bits1 == ulong.MaxValue ? 64 : iterated1 + MathUtils.TZC(~bits1);
					for (; iterated1 < runEnd1; iterated1++)
					{
						if ((resultBits.Bits1[current1] & (1UL << iterated1)) == 0UL)
						{
							continue;
						}

						Bits0Loop(offset1, iterated1, ref action);
					}
					continue;
				}

				Bits0Loop(offset1, iterated1, ref action);
				resultBits.Bits1[current1] &= bits1 - 1UL;

				while (resultBits.Bits1[current1] != 0UL)
				{
					bits1 = resultBits.Bits1[current1];
					iterated1 = MathUtils.TZC(bits1);
					Bits0Loop(offset1, iterated1, ref action);
					resultBits.Bits1[current1] &= bits1 - 1UL;
				}
			}

			dataSet1.PopRemoveOnRemove();
			dataSet2.PopRemoveOnRemove();

			BitsPool.Return(resultBits);
			return;

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			void Bits0Loop(int offset1, int iterated1, ref TAction action)
			{
				var current0 = offset1 + iterated1;
				var bits0 = resultBits.Bits0[current0];
				if (bits0 == 0UL)
				{
					return;
				}

				var dataOffset1 = dataSet1.Pages[current0].DataIndex & dataSet1.PageSizeMinusOne;
				var dataOffset2 = dataSet2.Pages[current0].DataIndex & dataSet2.PageSizeMinusOne;
				var dataPage1 = dataSet1.Data[dataSet1.Pages[current0].DataIndex >> dataSet1.PageSizePower];
				var dataPage2 = dataSet2.Data[dataSet2.Pages[current0].DataIndex >> dataSet2.PageSizePower];

				var offset0 = current0 << 6;
				var iterated0 = MathUtils.TZC(bits0);

				if (MathUtils.HasLessThen3Runs(bits0))
				{
					bits0 >>= iterated0;
					var runEnd0 = bits0 == ulong.MaxValue ? 64 : iterated0 + MathUtils.TZC(~bits0);
					for (; iterated0 < runEnd0; iterated0++)
					{
						if ((resultBits.Bits0[current0] & (1UL << iterated0)) == 0UL)
						{
							continue;
						}

						action.Apply(offset0 + iterated0,
							ref dataPage1[dataOffset1 + iterated0],
							ref dataPage2[dataOffset2 + iterated0]);
					}
					return;
				}

				action.Apply(offset0 + iterated0,
					ref dataPage1[dataOffset1 + iterated0],
					ref dataPage2[dataOffset2 + iterated0]);
				resultBits.Bits0[current0] &= bits0 - 1UL;

				while (resultBits.Bits0[current0] != 0UL)
				{
					bits0 = resultBits.Bits0[current0];
					iterated0 = MathUtils.TZC(bits0);
					action.Apply(offset0 + iterated0,
						ref dataPage1[dataOffset1 + iterated0],
						ref dataPage2[dataOffset2 + iterated0]);
					resultBits.Bits0[current0] &= bits0 - 1UL;
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

			var resultBits = BitsPool.RentClone(dataSet1).And(dataSet2).And(dataSet3);

			var bits1Length = BitsBase.GetMinBits(dataSet1, dataSet2, dataSet3).Bits1.Length;

			for (var current1 = 0; current1 < bits1Length; current1++)
			{
				var bits1 = resultBits.Bits1[current1];
				if (bits1 == 0UL)
				{
					continue;
				}

				var offset1 = current1 << 6;
				var iterated1 = MathUtils.TZC(bits1);

				if (MathUtils.HasLessThen3Runs(bits1))
				{
					bits1 >>= iterated1;
					var runEnd1 = bits1 == ulong.MaxValue ? 64 : iterated1 + MathUtils.TZC(~bits1);
					for (; iterated1 < runEnd1; iterated1++)
					{
						if ((resultBits.Bits1[current1] & (1UL << iterated1)) == 0UL)
						{
							continue;
						}

						Bits0Loop(offset1, iterated1, ref action);
					}
					continue;
				}

				Bits0Loop(offset1, iterated1, ref action);
				resultBits.Bits1[current1] &= bits1 - 1UL;

				while (resultBits.Bits1[current1] != 0UL)
				{
					bits1 = resultBits.Bits1[current1];
					iterated1 = MathUtils.TZC(bits1);
					Bits0Loop(offset1, iterated1, ref action);
					resultBits.Bits1[current1] &= bits1 - 1UL;
				}
			}

			dataSet1.PopRemoveOnRemove();
			dataSet2.PopRemoveOnRemove();
			dataSet3.PopRemoveOnRemove();
			
			BitsPool.Return(resultBits);
			return;

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			void Bits0Loop(int offset1, int iterated1, ref TAction action)
			{
				var current0 = offset1 + iterated1;
				var bits0 = resultBits.Bits0[current0];
				if (bits0 == 0UL)
				{
					return;
				}

				var dataOffset1 = dataSet1.Pages[current0].DataIndex & dataSet1.PageSizeMinusOne;
				var dataOffset2 = dataSet2.Pages[current0].DataIndex & dataSet2.PageSizeMinusOne;
				var dataOffset3 = dataSet3.Pages[current0].DataIndex & dataSet3.PageSizeMinusOne;
				var dataPage1 = dataSet1.Data[dataSet1.Pages[current0].DataIndex >> dataSet1.PageSizePower];
				var dataPage2 = dataSet2.Data[dataSet2.Pages[current0].DataIndex >> dataSet2.PageSizePower];
				var dataPage3 = dataSet3.Data[dataSet3.Pages[current0].DataIndex >> dataSet3.PageSizePower];

				var offset0 = current0 << 6;
				var iterated0 = MathUtils.TZC(bits0);

				if (MathUtils.HasLessThen3Runs(bits0))
				{
					bits0 >>= iterated0;
					var runEnd0 = bits0 == ulong.MaxValue ? 64 : iterated0 + MathUtils.TZC(~bits0);
					for (; iterated0 < runEnd0; iterated0++)
					{
						if ((resultBits.Bits0[current0] & (1UL << iterated0)) == 0UL)
						{
							continue;
						}

						action.Apply(offset0 + iterated0,
							ref dataPage1[dataOffset1 + iterated0],
							ref dataPage2[dataOffset2 + iterated0],
							ref dataPage3[dataOffset3 + iterated0]);
					}
					return;
				}

				action.Apply(offset0 + iterated0,
					ref dataPage1[dataOffset1 + iterated0],
					ref dataPage2[dataOffset2 + iterated0],
					ref dataPage3[dataOffset3 + iterated0]);
				resultBits.Bits0[current0] &= bits0 - 1UL;

				while (resultBits.Bits0[current0] != 0UL)
				{
					bits0 = resultBits.Bits0[current0];
					iterated0 = MathUtils.TZC(bits0);
					action.Apply(offset0 + iterated0,
						ref dataPage1[dataOffset1 + iterated0],
						ref dataPage2[dataOffset2 + iterated0],
						ref dataPage3[dataOffset3 + iterated0]);
					resultBits.Bits0[current0] &= bits0 - 1UL;
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

			var resultBits = BitsPool.RentClone(dataSet1).And(dataSet2).And(dataSet3).And(dataSet4);

			dataSet1.PushRemoveOnRemove(resultBits);
			dataSet2.PushRemoveOnRemove(resultBits);
			dataSet3.PushRemoveOnRemove(resultBits);
			dataSet4.PushRemoveOnRemove(resultBits);

			var bits1Length = BitsBase.GetMinBits(dataSet1, dataSet2, dataSet3, dataSet4).Bits1.Length;

			for (var current1 = 0; current1 < bits1Length; current1++)
			{
				var bits1 = resultBits.Bits1[current1];
				if (bits1 == 0UL)
				{
					continue;
				}

				var offset1 = current1 << 6;
				var iterated1 = MathUtils.TZC(bits1);

				if (MathUtils.HasLessThen3Runs(bits1))
				{
					bits1 >>= iterated1;
					var runEnd1 = bits1 == ulong.MaxValue ? 64 : iterated1 + MathUtils.TZC(~bits1);
					for (; iterated1 < runEnd1; iterated1++)
					{
						if ((resultBits.Bits1[current1] & (1UL << iterated1)) == 0UL)
						{
							continue;
						}

						Bits0Loop(offset1, iterated1, ref action);
					}
					continue;
				}

				Bits0Loop(offset1, iterated1, ref action);
				resultBits.Bits1[current1] &= bits1 - 1UL;

				while (resultBits.Bits1[current1] != 0UL)
				{
					bits1 = resultBits.Bits1[current1];
					iterated1 = MathUtils.TZC(bits1);
					Bits0Loop(offset1, iterated1, ref action);
					resultBits.Bits1[current1] &= bits1 - 1UL;
				}
			}

			dataSet1.PopRemoveOnRemove();
			dataSet2.PopRemoveOnRemove();
			dataSet3.PopRemoveOnRemove();
			dataSet4.PopRemoveOnRemove();
			
			BitsPool.Return(resultBits);
			return;

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			void Bits0Loop(int offset1, int iterated1, ref TAction action)
			{
				var current0 = offset1 + iterated1;
				var bits0 = resultBits.Bits0[current0];
				if (bits0 == 0UL)
				{
					return;
				}

				var dataOffset1 = dataSet1.Pages[current0].DataIndex & dataSet1.PageSizeMinusOne;
				var dataOffset2 = dataSet2.Pages[current0].DataIndex & dataSet2.PageSizeMinusOne;
				var dataOffset3 = dataSet3.Pages[current0].DataIndex & dataSet3.PageSizeMinusOne;
				var dataOffset4 = dataSet4.Pages[current0].DataIndex & dataSet4.PageSizeMinusOne;
				var dataPage1 = dataSet1.Data[dataSet1.Pages[current0].DataIndex >> dataSet1.PageSizePower];
				var dataPage2 = dataSet2.Data[dataSet2.Pages[current0].DataIndex >> dataSet2.PageSizePower];
				var dataPage3 = dataSet3.Data[dataSet3.Pages[current0].DataIndex >> dataSet3.PageSizePower];
				var dataPage4 = dataSet4.Data[dataSet4.Pages[current0].DataIndex >> dataSet4.PageSizePower];

				var offset0 = current0 << 6;
				var iterated0 = MathUtils.TZC(bits0);

				if (MathUtils.HasLessThen3Runs(bits0))
				{
					bits0 >>= iterated0;
					var runEnd0 = bits0 == ulong.MaxValue ? 64 : iterated0 + MathUtils.TZC(~bits0);
					for (; iterated0 < runEnd0; iterated0++)
					{
						if ((resultBits.Bits0[current0] & (1UL << iterated0)) == 0UL)
						{
							continue;
						}

						action.Apply(offset0 + iterated0,
							ref dataPage1[dataOffset1 + iterated0],
							ref dataPage2[dataOffset2 + iterated0],
							ref dataPage3[dataOffset3 + iterated0],
							ref dataPage4[dataOffset4 + iterated0]);
					}
					return;
				}

				action.Apply(offset0 + iterated0,
					ref dataPage1[dataOffset1 + iterated0],
					ref dataPage2[dataOffset2 + iterated0],
					ref dataPage3[dataOffset3 + iterated0],
					ref dataPage4[dataOffset4 + iterated0]);
				resultBits.Bits0[current0] &= bits0 - 1UL;

				while (resultBits.Bits0[current0] != 0UL)
				{
					bits0 = resultBits.Bits0[current0];
					iterated0 = MathUtils.TZC(bits0);
					action.Apply(offset0 + iterated0,
						ref dataPage1[dataOffset1 + iterated0],
						ref dataPage2[dataOffset2 + iterated0],
						ref dataPage3[dataOffset3 + iterated0],
						ref dataPage4[dataOffset4 + iterated0]);
					resultBits.Bits0[current0] &= bits0 - 1UL;
				}
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public BitsEnumerator GetEnumerator()
		{
			var bits = BitsPool.RentClone(Entifiers);
			Entifiers.PushRemoveOnRemove(bits);
			var pops = PopsPool.Rent().AddPopOnRemove(Entifiers);
			return new BitsEnumerator(bits, pops, Entifiers.Bits1.Length);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public EntityEnumerable Entities()
		{
			var bits = BitsPool.RentClone(Entifiers);
			Entifiers.PushRemoveOnRemove(bits);
			var pops = PopsPool.Rent().AddPopOnRemove(Entifiers);
			return new EntityEnumerable(bits, pops, this, Entifiers.Bits1.Length);
		}
	}
}
