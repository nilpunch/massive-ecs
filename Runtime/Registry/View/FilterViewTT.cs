using System.Linq;
using System.Runtime.CompilerServices;
using Unity.IL2CPP.CompilerServices;

namespace Massive
{
	[Il2CppSetOption(Option.NullChecks, false)]
	[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
	public class FilterView<T1, T2>
		where T1 : struct
		where T2 : struct
	{
		private readonly IFilter _filter;
		private readonly IReadOnlyDataSet<T1> _components1;
		private readonly IReadOnlyDataSet<T2> _components2;
		private readonly IReadOnlySet[] _componentsAndInclude;

		public FilterView(IRegistry registry, IFilter filter = null)
		{
			_filter = filter ?? EmptyFilter.Instance;
			_components1 = registry.Components<T1>();
			_components2 = registry.Components<T2>();
			_componentsAndInclude = Enumerable.Empty<IReadOnlySet>()
				.Append(_components1)
				.Append(_components2)
				.Concat(_filter.Include)
				.ToArray();
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void ForEach(EntityActionRef<T1, T2> action)
		{
			var data1 = _components1.AliveData;
			var data2 = _components2.AliveData;
			var ids = SetHelpers.GetMinimalSet(_componentsAndInclude).AliveIds;

			for (int i = ids.Length - 1; i >= 0; i--)
			{
				int id = ids[i];
				if (_components1.TryGetDense(id, out var dense1)
				    && _components2.TryGetDense(id, out var dense2))
				{
					if (_filter.ContainsId(id))
					{
						action.Invoke(id, ref data1[dense1], ref data2[dense2]);
					}
				}
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void ForEachExtra<TExtra>(TExtra extra, EntityActionRefExtra<T1, T2, TExtra> action)
		{
			var data1 = _components1.AliveData;
			var data2 = _components2.AliveData;
			var ids = SetHelpers.GetMinimalSet(_componentsAndInclude).AliveIds;

			for (int i = ids.Length - 1; i >= 0; i--)
			{
				int id = ids[i];
				if (_components1.TryGetDense(id, out var dense1)
				    && _components2.TryGetDense(id, out var dense2))
				{
					if (_filter.ContainsId(id))
					{
						action.Invoke(id, ref data1[dense1], ref data2[dense2], extra);
					}
				}
			}
		}
	}
}