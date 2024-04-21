using System.Runtime.CompilerServices;
using Unity.IL2CPP.CompilerServices;

namespace Massive
{
	[Il2CppSetOption(Option.NullChecks, false)]
	[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
	public readonly struct FilterView<T1, T2, T3>
		where T1 : struct
		where T2 : struct
		where T3 : struct
	{
		private readonly IFilter _filter;
		private readonly IReadOnlyDataSet<T1> _components1;
		private readonly IReadOnlyDataSet<T2> _components2;
		private readonly IReadOnlyDataSet<T3> _components3;

		public FilterView(IRegistry registry, IFilter filter = null)
		{
			_filter = filter ?? EmptyFilter.Instance;
			_components1 = registry.Components<T1>();
			_components2 = registry.Components<T2>();
			_components3 = registry.Components<T3>();
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void ForEach(EntityActionRef<T1, T2, T3> action)
		{
			var data1 = _components1.Data;
			var data2 = _components2.Data;
			var data3 = _components3.Data;
			var minData = SetHelpers.GetMinimalSet(_components1, _components2, _components3);
			var ids = SetHelpers.GetMinimalSet(minData, _filter.Include).Ids;

			for (int i = ids.Length - 1; i >= 0; i--)
			{
				int id = ids[i];
				if (_components1.TryGetDense(id, out var dense1)
				    && _components2.TryGetDense(id, out var dense2)
				    && _components3.TryGetDense(id, out var dense3))
				{
					if (_filter.ContainsId(id))
					{
						action.Invoke(id, ref data1[dense1], ref data2[dense2], ref data3[dense3]);
					}
				}
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void ForEachExtra<TExtra>(TExtra extra, EntityActionRefExtra<T1, T2, T3, TExtra> action)
		{
			var data1 = _components1.Data;
			var data2 = _components2.Data;
			var data3 = _components3.Data;
			var minData = SetHelpers.GetMinimalSet(_components1, _components2, _components3);
			var ids = SetHelpers.GetMinimalSet(minData, _filter.Include).Ids;

			for (int i = ids.Length - 1; i >= 0; i--)
			{
				int id = ids[i];
				if (_components1.TryGetDense(id, out var dense1)
				    && _components2.TryGetDense(id, out var dense2)
				    && _components3.TryGetDense(id, out var dense3))
				{
					if (_filter.ContainsId(id))
					{
						action.Invoke(id, ref data1[dense1], ref data2[dense2], ref data3[dense3], extra);
					}
				}
			}
		}
	}
}