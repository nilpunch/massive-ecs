using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
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

		public void ForEach<TAction>(TAction action)
			where TAction : IEntityAction
		{
			if (Filter.Include.Count == 0)
			{
				var entities = Registry.Entities.Alive;

				for (var i = entities.Length - 1; i >= 0; i--)
				{
					var entity = entities[i];
					if (Filter.ContainsId(entity.Id))
					{
						action.Apply(entity.Id);
					}
				}
			}
			else
			{
				var ids = SetHelpers.GetMinimalSet(Filter.Include).Ids;

				for (var i = ids.Length - 1; i >= 0; i--)
				{
					var id = ids[i];
					if (Filter.ContainsId(id))
					{
						action.Apply(id);
					}
				}
			}
		}

		public void ForEach<TAction, T>(TAction action)
			where TAction : IEntityAction<T>
		{
			var dataSet = Registry.DataSet<T>();

			var data = dataSet.Data;
			var ids = SetHelpers.GetMinimalSet(dataSet, Filter.Include).Ids;

			for (int i = ids.Length - 1; i >= 0; i--)
			{
				var id = ids[i];
				var dense = dataSet.GetDenseOrInvalid(id);
				if (dense != Constants.InvalidId && Filter.ContainsId(id))
				{
					action.Apply(id, ref data[dense]);
				}
			}
		}

		public void ForEach<TAction, T1, T2>(TAction action)
			where TAction : IEntityAction<T1, T2>
		{
			var dataSet1 = Registry.DataSet<T1>();
			var dataSet2 = Registry.DataSet<T2>();

			var data1 = dataSet1.Data;
			var data2 = dataSet2.Data;
			var minData = SetHelpers.GetMinimalSet(dataSet1, dataSet2);
			var ids = SetHelpers.GetMinimalSet(minData, Filter.Include).Ids;

			for (int i = ids.Length - 1; i >= 0; i--)
			{
				var id = ids[i];
				var dense1 = dataSet1.GetDenseOrInvalid(id);
				var dense2 = dataSet2.GetDenseOrInvalid(id);
				if (dense1 != Constants.InvalidId
				    && dense2 != Constants.InvalidId
				    && Filter.ContainsId(id))
				{
					action.Apply(id, ref data1[dense1], ref data2[dense2]);
				}
			}
		}

		public void ForEach<TAction, T1, T2, T3>(TAction action)
			where TAction : IEntityAction<T1, T2, T3>
		{
			var dataSet1 = Registry.DataSet<T1>();
			var dataSet2 = Registry.DataSet<T2>();
			var dataSet3 = Registry.DataSet<T3>();

			var data1 = dataSet1.Data;
			var data2 = dataSet2.Data;
			var data3 = dataSet3.Data;
			var minData = SetHelpers.GetMinimalSet(dataSet1, dataSet2, dataSet3);
			var ids = SetHelpers.GetMinimalSet(minData, Filter.Include).Ids;

			for (int i = ids.Length - 1; i >= 0; i--)
			{
				var id = ids[i];
				var dense1 = dataSet1.GetDenseOrInvalid(id);
				var dense2 = dataSet2.GetDenseOrInvalid(id);
				var dense3 = dataSet3.GetDenseOrInvalid(id);
				if (dense1 != Constants.InvalidId
				    && dense2 != Constants.InvalidId
				    && dense3 != Constants.InvalidId
				    && Filter.ContainsId(id))
				{
					action.Apply(id, ref data1[dense1], ref data2[dense2], ref data3[dense3]);
				}
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Enumerator GetEnumerator()
		{
			if (Filter.Include.Count == 0)
			{
				return new Enumerator(Registry.Entities.Alive, Filter);
			}
			else
			{
				var ids = SetHelpers.GetMinimalSet(Filter.Include).Ids;
				return new Enumerator(ids, Filter);
			}
		}

		public ref struct Enumerator
		{
			private readonly ReadOnlySpan<int> _ids;
			private readonly IFilter _filter;
			private readonly int _stride;
			private readonly int _idOffset;
			private int _index;

			public Enumerator(ReadOnlySpan<Entity> entities, IFilter filter)
				: this(MemoryMarshal.Cast<Entity, int>(entities), filter, 2, Entity.IdOffset)
			{
			}

			public Enumerator(ReadOnlySpan<int> ids, IFilter filter)
				: this(ids, filter, 1, 0)
			{
			}

			private Enumerator(ReadOnlySpan<int> ids, IFilter filter, int stride, int idOffset)
			{
				_ids = ids;
				_filter = filter;
				_stride = stride;
				_idOffset = idOffset;
				_index = _ids.Length / _stride;
			}

			public int Current
			{
				[MethodImpl(MethodImplOptions.AggressiveInlining)]
				get => _ids[_index * _stride] - _idOffset;
			}

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public bool MoveNext()
			{
				while (--_index >= 0 && !_filter.ContainsId(Current))
				{
				}

				return _index >= 0;
			}
		}
	}
}
