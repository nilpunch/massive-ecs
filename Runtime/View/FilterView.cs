using System.Runtime.CompilerServices;
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
		public void ForEachUniversal<TInvoker>(TInvoker invoker)
			where TInvoker : IEntityActionInvoker
		{
			if (Filter.Include.Count == 0)
			{
				var entities = Registry.Entities.Alive;

				for (var i = entities.Length - 1; i >= 0; i--)
				{
					var entity = entities[i];
					if (Filter.ContainsId(entity.Id))
					{
						invoker.Apply(entity.Id);
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
						invoker.Apply(id);
					}
				}
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void ForEachUniversal<TInvoker, T>(TInvoker invoker)
			where TInvoker : IEntityActionInvoker<T>
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
					invoker.Apply(id, ref data[dense]);
				}
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void ForEachUniversal<TInvoker, T1, T2>(TInvoker invoker)
			where TInvoker : IEntityActionInvoker<T1, T2>
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
					invoker.Apply(id, ref data1[dense1], ref data2[dense2]);
				}
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void ForEachUniversal<TInvoker, T1, T2, T3>(TInvoker invoker)
			where TInvoker : IEntityActionInvoker<T1, T2, T3>
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
					invoker.Apply(id, ref data1[dense1], ref data2[dense2], ref data3[dense3]);
				}
			}
		}
	}
}
