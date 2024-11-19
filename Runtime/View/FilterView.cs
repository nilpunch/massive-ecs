using System;
using System.Runtime.CompilerServices;
using Unity.IL2CPP.CompilerServices;

namespace Massive
{
	[Il2CppSetOption(Option.NullChecks, false)]
	[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
	[Il2CppSetOption(Option.DivideByZeroChecks, false)]
	public readonly struct FilterView : IView
	{
		public Registry Registry { get; }
		public Filter Filter { get; }
		public Packing PackingWhenIterating { get; }

		public FilterView(Registry registry, Filter filter = default,
			Packing packingWhenIterating = Packing.WithHoles)
		{
			Registry = registry;
			PackingWhenIterating = packingWhenIterating;
			Filter = filter.IsValid() ? filter : Filter.Empty;
		}

		public void ForEach<TAction>(ref TAction action)
			where TAction : IEntityAction
		{
			IdsSource idsSource = Filter.Included.Length == 0
				? Registry.Entities
				: SetUtils.GetMinimalSet(Filter.Included);

			var originalPacking = idsSource.ExchangeToStricterPacking(PackingWhenIterating);

			for (var i = idsSource.Count - 1; i >= 0; i--)
			{
				if (i > idsSource.Count)
				{
					i = idsSource.Count;
					continue;
				}

				var id = idsSource.Ids[i];
				if (Filter.ContainsId(id))
				{
					if (!action.Apply(id))
					{
						break;
					}
				}
			}

			idsSource.ExchangePacking(originalPacking);
		}

		public void ForEach<TAction, T>(ref TAction action)
			where TAction : IEntityAction<T>
		{
			var dataSet = Registry.DataSet<T>();

			ThrowIfCantInclude(dataSet);

			var data = dataSet.Data;

			var minSet = SetUtils.GetMinimalSet(dataSet, Filter.Included);
			var originalPacking = minSet.ExchangeToStricterPacking(PackingWhenIterating);

			for (var i = minSet.Count - 1; i >= 0; i--)
			{
				if (i > minSet.Count)
				{
					i = minSet.Count;
					continue;
				}

				var id = minSet.Ids[i];
				var index = dataSet.GetIndexOrInvalid(id);
				if (index >= 0 && Filter.ContainsId(id))
				{
					if (!action.Apply(id, ref data[index]))
					{
						break;
					}
				}
			}

			minSet.ExchangePacking(originalPacking);
		}

		public void ForEach<TAction, T1, T2>(ref TAction action)
			where TAction : IEntityAction<T1, T2>
		{
			var dataSet1 = Registry.DataSet<T1>();
			var dataSet2 = Registry.DataSet<T2>();

			ThrowIfCantInclude(dataSet1);
			ThrowIfCantInclude(dataSet2);

			var data1 = dataSet1.Data;
			var data2 = dataSet2.Data;
			var minDataSet = SetUtils.GetMinimalSet(dataSet1, dataSet2);

			var minSet = SetUtils.GetMinimalSet(minDataSet, Filter.Included);
			var originalPacking = minSet.ExchangeToStricterPacking(PackingWhenIterating);

			for (var i = minSet.Count - 1; i >= 0; i--)
			{
				if (i > minSet.Count)
				{
					i = minSet.Count;
					continue;
				}

				var id = minSet.Ids[i];
				var index1 = dataSet1.GetIndexOrInvalid(id);
				var index2 = dataSet2.GetIndexOrInvalid(id);
				if ((index1 | index2) >= 0
				    && Filter.ContainsId(id))
				{
					if (!action.Apply(id, ref data1[index1], ref data2[index2]))
					{
						break;
					}
				}
			}

			minSet.ExchangePacking(originalPacking);
		}

		public void ForEach<TAction, T1, T2, T3>(ref TAction action)
			where TAction : IEntityAction<T1, T2, T3>
		{
			var dataSet1 = Registry.DataSet<T1>();
			var dataSet2 = Registry.DataSet<T2>();
			var dataSet3 = Registry.DataSet<T3>();

			ThrowIfCantInclude(dataSet1);
			ThrowIfCantInclude(dataSet2);
			ThrowIfCantInclude(dataSet3);

			var data1 = dataSet1.Data;
			var data2 = dataSet2.Data;
			var data3 = dataSet3.Data;
			var minDataSet = SetUtils.GetMinimalSet(dataSet1, dataSet2, dataSet3);

			var minSet = SetUtils.GetMinimalSet(minDataSet, Filter.Included);
			var originalPacking = minSet.ExchangeToStricterPacking(PackingWhenIterating);

			for (var i = minSet.Count - 1; i >= 0; i--)
			{
				if (i > minSet.Count)
				{
					i = minSet.Count;
					continue;
				}

				var id = minSet.Ids[i];
				var index1 = dataSet1.GetIndexOrInvalid(id);
				var index2 = dataSet2.GetIndexOrInvalid(id);
				var index3 = dataSet3.GetIndexOrInvalid(id);
				if ((index1 | index2 | index3) >= 0
				    && Filter.ContainsId(id))
				{
					if (!action.Apply(id, ref data1[index1], ref data2[index2], ref data3[index3]))
					{
						break;
					}
				}
			}

			minSet.ExchangePacking(originalPacking);
		}

		public void ForEach<TAction, T1, T2, T3, T4>(ref TAction action)
			where TAction : IEntityAction<T1, T2, T3, T4>
		{
			var dataSet1 = Registry.DataSet<T1>();
			var dataSet2 = Registry.DataSet<T2>();
			var dataSet3 = Registry.DataSet<T3>();
			var dataSet4 = Registry.DataSet<T4>();

			ThrowIfCantInclude(dataSet1);
			ThrowIfCantInclude(dataSet2);
			ThrowIfCantInclude(dataSet3);
			ThrowIfCantInclude(dataSet4);

			var data1 = dataSet1.Data;
			var data2 = dataSet2.Data;
			var data3 = dataSet3.Data;
			var data4 = dataSet4.Data;
			var minDataSet = SetUtils.GetMinimalSet(dataSet1, dataSet2, dataSet3, dataSet4);

			var minSet = SetUtils.GetMinimalSet(minDataSet, Filter.Included);
			var originalPacking = minSet.ExchangeToStricterPacking(PackingWhenIterating);

			for (var i = minSet.Count - 1; i >= 0; i--)
			{
				if (i > minSet.Count)
				{
					i = minSet.Count;
					continue;
				}

				var id = minSet.Ids[i];
				var index1 = dataSet1.GetIndexOrInvalid(id);
				var index2 = dataSet2.GetIndexOrInvalid(id);
				var index3 = dataSet3.GetIndexOrInvalid(id);
				var index4 = dataSet4.GetIndexOrInvalid(id);
				if ((index1 | index2 | index3 | index4) >= 0
				    && Filter.ContainsId(id))
				{
					if (!action.Apply(id, ref data1[index1], ref data2[index2], ref data3[index3], ref data4[index4]))
					{
						break;
					}
				}
			}

			minSet.ExchangePacking(originalPacking);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public IdsFilterEnumerator GetEnumerator()
		{
			IdsSource idsSource = Filter.Included.Length == 0
				? Registry.Entities
				: SetUtils.GetMinimalSet(Filter.Included);

			return new IdsFilterEnumerator(idsSource, Filter, PackingWhenIterating);
		}

		private void ThrowIfCantInclude(SparseSet sparseSet)
		{
			if (Filter.Excluded.Contains(sparseSet))
			{
				throw new Exception($"Conflicting exclude filter and {sparseSet.GetType().GetGenericName()}!");
			}
		}
	}
}
