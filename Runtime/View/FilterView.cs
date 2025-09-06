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
			var rentedPops = PopsPool.Rent();

			int bits1Length;

			if (Filter.IncludedCount == 0)
			{
				World.Entifiers.CopyBitsTo(resultBits);
				World.Entifiers.PushRemoveOnRemove(resultBits);
				rentedPops.AddPopOnRemove(World.Entifiers);
				bits1Length = World.Entifiers.Bits1.Length;
			}
			else
			{
				var minBits = BitsBase.GetMinBits(Filter.Included, Filter.IncludedCount);
				minBits.CopyBitsTo(resultBits);
				bits1Length = minBits.Bits1.Length;
			}

			FilterBitsAndPops(resultBits, rentedPops);

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

			BitsPool.Return(resultBits);
			PopsPool.ReturnAndPop(rentedPops);
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
			NoDataException.ThrowIfHasNoData<T>(World, DataAccessContext.View);

			var dataSet = World.DataSet<T>();

			var resultBits = BitsPool.Rent();
			var rentedPops = PopsPool.Rent();

			dataSet.CopyBitsTo(resultBits);

			dataSet.PushRemoveOnRemove(resultBits);
			rentedPops.AddPopOnRemove(dataSet);

			FilterBitsAndPops(resultBits, rentedPops);

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

			BitsPool.Return(resultBits);
			PopsPool.ReturnAndPop(rentedPops);
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
			NoDataException.ThrowIfHasNoData<T1>(World, DataAccessContext.View);
			NoDataException.ThrowIfHasNoData<T2>(World, DataAccessContext.View);

			var dataSet1 = World.DataSet<T1>();
			var dataSet2 = World.DataSet<T2>();

			var minSet = BitsBase.GetMinBits(dataSet1, dataSet2);

			var resultBits = BitsPool.RentClone(minSet);
			var rentedPops = PopsPool.Rent();

			resultBits.And(dataSet1).And(dataSet2);

			dataSet1.PushRemoveOnRemove(resultBits);
			dataSet2.PushRemoveOnRemove(resultBits);
			rentedPops.AddPopOnRemove(dataSet1);
			rentedPops.AddPopOnRemove(dataSet2);

			FilterBitsAndPops(resultBits, rentedPops);

			var bits1Length = minSet.Bits1.Length;

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

			BitsPool.Return(resultBits);
			PopsPool.ReturnAndPop(rentedPops);
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
			NoDataException.ThrowIfHasNoData<T1>(World, DataAccessContext.View);
			NoDataException.ThrowIfHasNoData<T2>(World, DataAccessContext.View);
			NoDataException.ThrowIfHasNoData<T3>(World, DataAccessContext.View);

			var dataSet1 = World.DataSet<T1>();
			var dataSet2 = World.DataSet<T2>();
			var dataSet3 = World.DataSet<T3>();

			var minSet = BitsBase.GetMinBits(dataSet1, dataSet2, dataSet3);

			var resultBits = BitsPool.RentClone(minSet);
			var rentedPops = PopsPool.Rent();

			resultBits.And(dataSet1).And(dataSet2).And(dataSet3);

			dataSet1.PushRemoveOnRemove(resultBits);
			dataSet2.PushRemoveOnRemove(resultBits);
			dataSet3.PushRemoveOnRemove(resultBits);
			rentedPops.AddPopOnRemove(dataSet1);
			rentedPops.AddPopOnRemove(dataSet2);
			rentedPops.AddPopOnRemove(dataSet3);

			FilterBitsAndPops(resultBits, rentedPops);

			var bits1Length = minSet.Bits1.Length;

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

			BitsPool.Return(resultBits);
			PopsPool.ReturnAndPop(rentedPops);
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
			NoDataException.ThrowIfHasNoData<T1>(World, DataAccessContext.View);
			NoDataException.ThrowIfHasNoData<T2>(World, DataAccessContext.View);
			NoDataException.ThrowIfHasNoData<T3>(World, DataAccessContext.View);
			NoDataException.ThrowIfHasNoData<T4>(World, DataAccessContext.View);

			var dataSet1 = World.DataSet<T1>();
			var dataSet2 = World.DataSet<T2>();
			var dataSet3 = World.DataSet<T3>();
			var dataSet4 = World.DataSet<T4>();

			var minSet = BitsBase.GetMinBits(dataSet1, dataSet2, dataSet3, dataSet4);

			var resultBits = BitsPool.RentClone(minSet);
			var rentedPops = PopsPool.Rent();

			resultBits.And(dataSet1).And(dataSet2).And(dataSet3).And(dataSet4);

			dataSet1.PushRemoveOnRemove(resultBits);
			dataSet2.PushRemoveOnRemove(resultBits);
			dataSet3.PushRemoveOnRemove(resultBits);
			dataSet4.PushRemoveOnRemove(resultBits);
			rentedPops.AddPopOnRemove(dataSet1);
			rentedPops.AddPopOnRemove(dataSet2);
			rentedPops.AddPopOnRemove(dataSet3);
			rentedPops.AddPopOnRemove(dataSet4);

			FilterBitsAndPops(resultBits, rentedPops);

			var bits1Length = minSet.Bits1.Length;

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

			BitsPool.Return(resultBits);
			PopsPool.ReturnAndPop(rentedPops);
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
			var resultBits = BitsPool.Rent();
			var rentedPops = PopsPool.Rent();

			int bits1Length;

			if (Filter.IncludedCount == 0)
			{
				World.Entifiers.CopyBitsTo(resultBits);
				World.Entifiers.PushRemoveOnRemove(resultBits);
				rentedPops.AddPopOnRemove(World.Entifiers);
				bits1Length = World.Entifiers.Bits1.Length;
			}
			else
			{
				var minBits = BitsBase.GetMinBits(Filter.Included, Filter.IncludedCount);
				minBits.CopyBitsTo(resultBits);
				bits1Length = minBits.Bits1.Length;
			}

			FilterBitsAndPops(resultBits, rentedPops);

			return new BitsEnumerator(resultBits, rentedPops, bits1Length);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public EntityEnumerable Entities()
		{
			var resultBits = BitsPool.Rent();
			var rentedPops = PopsPool.Rent();

			int bits1Length;

			if (Filter.IncludedCount == 0)
			{
				World.Entifiers.CopyBitsTo(resultBits);
				World.Entifiers.PushRemoveOnRemove(resultBits);
				rentedPops.AddPopOnRemove(World.Entifiers);
				bits1Length = World.Entifiers.Bits1.Length;
			}
			else
			{
				var minBits = BitsBase.GetMinBits(Filter.Included, Filter.IncludedCount);
				minBits.CopyBitsTo(resultBits);
				bits1Length = minBits.Bits1.Length;
			}

			FilterBitsAndPops(resultBits, rentedPops);

			return new EntityEnumerable(resultBits, rentedPops, World, bits1Length);
		}

		private void FilterBitsAndPops(Bits resultBits, Pops rentedPops)
		{
			for (var i = 0; i < Filter.IncludedCount; i++)
			{
				var included = Filter.Included[i];
				resultBits.And(included);
				included.PushRemoveOnRemove(resultBits);
				rentedPops.AddPopOnRemove(included);
			}

			for (var i = 0; i < Filter.ExcludedCount; i++)
			{
				var excluded = Filter.Excluded[i];
				resultBits.Not(excluded);
				excluded.PushRemoveOnAdd(resultBits);
				rentedPops.AddPopOnAdd(excluded);
			}
		}
	}
}
