using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Unity.IL2CPP.CompilerServices;

namespace Massive
{
	[Il2CppSetOption(Option.NullChecks, false)]
	[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
	public readonly struct FilterView : IView
	{
		private IFilter Filter { get; }

		public IRegistry Registry { get; }

		public FilterView(IRegistry registry, IFilter filter = null)
		{
			Registry = registry;
			Filter = filter ?? EmptyFilter.Instance;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
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

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void ForEach<TAction, T>(TAction action)
			where TAction : IEntityAction<T>
		{
			var components = Registry.Components<T>();

			var data = components.Data;
			var ids = SetHelpers.GetMinimalSet(components, Filter.Include).Ids;

			for (int i = ids.Length - 1; i >= 0; i--)
			{
				var id = ids[i];
				var dense = components.GetDenseOrInvalid(id);
				if (dense != Constants.InvalidId && Filter.ContainsId(id))
				{
					action.Apply(id, ref data[dense]);
				}
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void ForEach<TAction, T1, T2>(TAction action)
			where TAction : IEntityAction<T1, T2>
		{
			var components1 = Registry.Components<T1>();
			var components2 = Registry.Components<T2>();

			var data1 = components1.Data;
			var data2 = components2.Data;
			var minData = SetHelpers.GetMinimalSet(components1, components2);
			var ids = SetHelpers.GetMinimalSet(minData, Filter.Include).Ids;

			for (int i = ids.Length - 1; i >= 0; i--)
			{
				var id = ids[i];
				var dense1 = components1.GetDenseOrInvalid(id);
				var dense2 = components2.GetDenseOrInvalid(id);
				if (dense1 != Constants.InvalidId
				    && dense2 != Constants.InvalidId
				    && Filter.ContainsId(id))
				{
					action.Apply(id, ref data1[dense1], ref data2[dense2]);
				}
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void ForEach<TAction, T1, T2, T3>(TAction action)
			where TAction : IEntityAction<T1, T2, T3>
		{
			var components1 = Registry.Components<T1>();
			var components2 = Registry.Components<T2>();
			var components3 = Registry.Components<T3>();

			var data1 = components1.Data;
			var data2 = components2.Data;
			var data3 = components3.Data;
			var minData = SetHelpers.GetMinimalSet(components1, components2, components3);
			var ids = SetHelpers.GetMinimalSet(minData, Filter.Include).Ids;

			for (int i = ids.Length - 1; i >= 0; i--)
			{
				var id = ids[i];
				var dense1 = components1.GetDenseOrInvalid(id);
				var dense2 = components2.GetDenseOrInvalid(id);
				var dense3 = components3.GetDenseOrInvalid(id);
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
