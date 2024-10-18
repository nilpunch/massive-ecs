using System.Runtime.CompilerServices;
using Unity.IL2CPP.CompilerServices;

namespace Massive
{
	[Il2CppSetOption(Option.NullChecks | Option.ArrayBoundsChecks, false)]
	public readonly struct FilterView : IView
	{
		private IFilter Filter { get; }

		public Registry Registry { get; }

		public FilterView(Registry registry, IFilter filter = null)
		{
			Registry = registry;
			Filter = filter ?? EmptyFilter.Instance;
		}

		public void ForEach<TAction>(ref TAction action)
			where TAction : IEntityAction
		{
			IIdsSource idsSource = Filter.Include.Count == 0
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
				var packed = dataSet.GetIndexOrInvalid(id);
				if (packed >= 0 && Filter.ContainsId(id))
				{
					if (!action.Apply(id, ref data[packed]))
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
				var packed1 = dataSet1.GetIndexOrInvalid(id);
				var packed2 = dataSet2.GetIndexOrInvalid(id);
				if (packed1 >= 0
				    && packed2 >= 0
				    && Filter.ContainsId(id))
				{
					if (!action.Apply(id, ref data1[packed1], ref data2[packed2]))
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
				var packed1 = dataSet1.GetIndexOrInvalid(id);
				var packed2 = dataSet2.GetIndexOrInvalid(id);
				var packed3 = dataSet3.GetIndexOrInvalid(id);
				if (packed1 >= 0
				    && packed2 >= 0
				    && packed3 >= 0
				    && Filter.ContainsId(id))
				{
					if (!action.Apply(id, ref data1[packed1], ref data2[packed2], ref data3[packed3]))
					{
						break;
					}
				}
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Enumerator GetEnumerator()
		{
			if (Filter.Include.Count == 0)
			{
				return new Enumerator(Registry.Entities, Filter);
			}
			else
			{
				var ids = SetHelpers.GetMinimalSet(Filter.Include);
				return new Enumerator(ids, Filter);
			}
		}

		public ref struct Enumerator
		{
			private readonly IIdsSource _idsSource;
			private readonly IFilter _filter;
			private int _index;

			public Enumerator(IIdsSource idsSource, IFilter filter)
			{
				_idsSource = idsSource;
				_filter = filter;
				_index = _idsSource.Count;
			}

			public int Current
			{
				[MethodImpl(MethodImplOptions.AggressiveInlining)]
				get => _idsSource.Ids[_index];
			}

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public bool MoveNext()
			{
				if (--_index > _idsSource.Count)
				{
					_index = _idsSource.Count - 1;
				}

				while (_index >= 0 && !_filter.ContainsId(Current))
				{
					--_index;
				}

				return _index >= 0;
			}
		}
	}
}
