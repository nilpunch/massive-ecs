using System.Runtime.CompilerServices;
using Unity.IL2CPP.CompilerServices;

namespace Massive
{
	[Il2CppSetOption(Option.NullChecks | Option.ArrayBoundsChecks, false)]
	public readonly struct FilterView : IView
	{
		private Filter Filter { get; }

		public Registry Registry { get; }

		public FilterView(Registry registry, Filter filter = null)
		{
			Registry = registry;
			Filter = filter ?? Filter.Empty;
		}

		public void ForEach<TAction>(ref TAction action)
			where TAction : IEntityAction
		{
			IdsSource idsSource = Filter.Include.Length == 0
				? Registry.Entities
				: SetHelpers.GetMinimalSet(Filter.Include);

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
		}

		public void ForEach<TAction, T>(ref TAction action)
			where TAction : IEntityAction<T>
		{
			var dataSet = Registry.DataSet<T>();

			var data = dataSet.Data;
			var set = SetHelpers.GetMinimalSet(dataSet, Filter.Include);

			for (int i = set.Count - 1; i >= 0; i--)
			{
				if (i > set.Count)
				{
					i = set.Count;
					continue;
				}

				var id = set.Ids[i];
				var index = dataSet.GetIndexOrInvalid(id);
				if (index >= 0 && Filter.ContainsId(id))
				{
					if (!action.Apply(id, ref data[index]))
					{
						break;
					}
				}
			}
		}

		public void ForEach<TAction, T1, T2>(ref TAction action)
			where TAction : IEntityAction<T1, T2>
		{
			var dataSet1 = Registry.DataSet<T1>();
			var dataSet2 = Registry.DataSet<T2>();

			var data1 = dataSet1.Data;
			var data2 = dataSet2.Data;
			var minData = SetHelpers.GetMinimalSet(dataSet1, dataSet2);
			var set = SetHelpers.GetMinimalSet(minData, Filter.Include);

			for (int i = set.Count - 1; i >= 0; i--)
			{
				if (i > set.Count)
				{
					i = set.Count;
					continue;
				}

				var id = set.Ids[i];
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
		}

		public void ForEach<TAction, T1, T2, T3>(ref TAction action)
			where TAction : IEntityAction<T1, T2, T3>
		{
			var dataSet1 = Registry.DataSet<T1>();
			var dataSet2 = Registry.DataSet<T2>();
			var dataSet3 = Registry.DataSet<T3>();

			var data1 = dataSet1.Data;
			var data2 = dataSet2.Data;
			var data3 = dataSet3.Data;
			var minData = SetHelpers.GetMinimalSet(dataSet1, dataSet2, dataSet3);
			var set = SetHelpers.GetMinimalSet(minData, Filter.Include);

			for (int i = set.Count - 1; i >= 0; i--)
			{
				if (i > set.Count)
				{
					i = set.Count;
					continue;
				}

				var id = set.Ids[i];
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
		}

		public void ForEach<TAction, T1, T2, T3, T4>(ref TAction action)
			where TAction : IEntityAction<T1, T2, T3, T4>
		{
			var dataSet1 = Registry.DataSet<T1>();
			var dataSet2 = Registry.DataSet<T2>();
			var dataSet3 = Registry.DataSet<T3>();
			var dataSet4 = Registry.DataSet<T4>();

			var data1 = dataSet1.Data;
			var data2 = dataSet2.Data;
			var data3 = dataSet3.Data;
			var data4 = dataSet4.Data;
			var minData = SetHelpers.GetMinimalSet(dataSet1, dataSet2, dataSet3, dataSet4);
			var set = SetHelpers.GetMinimalSet(minData, Filter.Include);

			for (int i = set.Count - 1; i >= 0; i--)
			{
				if (i > set.Count)
				{
					i = set.Count;
					continue;
				}

				var id = set.Ids[i];
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
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public IdsSourceFilterEnumerator GetEnumerator()
		{
			if (Filter.Include.Length == 0)
			{
				return new IdsSourceFilterEnumerator(Registry.Entities, Filter);
			}
			else
			{
				var ids = SetHelpers.GetMinimalSet(Filter.Include);
				return new IdsSourceFilterEnumerator(ids, Filter);
			}
		}
	}
}
