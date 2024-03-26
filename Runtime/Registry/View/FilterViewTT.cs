using System;
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
		private readonly Filter _filter;
		private readonly IDataSet<T1> _components1;
		private readonly IDataSet<T2> _components2;
		private readonly ISet[] _componentsAndInclude;

		public FilterView(IRegistry registry, Filter filter)
		{
			_filter = filter;
			_components1 = registry.Components<T1>();
			_components2 = registry.Components<T2>();
			_componentsAndInclude = new ISet[_filter.Include.Length + 2];
			_componentsAndInclude[0] = _components1;
			_componentsAndInclude[1] = _components2;
			_filter.Include.CopyTo(_componentsAndInclude, 2);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void ForEach(EntityActionRef<T1, T2> action)
		{
			var data1 = _components1.AliveData;
			var data2 = _components2.AliveData;
			var ids = SetUtils.GetMinimalSet(_componentsAndInclude).AliveIds;

			for (int i = ids.Length - 1; i >= 0; i--)
			{
				int id = ids[i];
				if (_components1.TryGetDense(id, out var dense1)
				    && _components2.TryGetDense(id, out var dense2))
				{
					if (_filter.Contains(id))
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
			var ids = SetUtils.GetMinimalSet(_componentsAndInclude).AliveIds;

			for (int i = ids.Length - 1; i >= 0; i--)
			{
				int id = ids[i];
				if (_components1.TryGetDense(id, out var dense1)
				    && _components2.TryGetDense(id, out var dense2))
				{
					if (_filter.Contains(id))
					{
						action.Invoke(id, ref data1[dense1], ref data2[dense2], extra);
					}
				}
			}
		}
	}
}