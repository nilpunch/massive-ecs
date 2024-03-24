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
		private readonly IDataSet<T1> _components1;
		private readonly IDataSet<T2> _components2;
		private readonly ISet[] _include;
		private readonly ISet[] _exclude;
		private readonly ISet[] _componentsAndInclude;

		public FilterView(IRegistry registry, ISet[] include = null, ISet[] exclude = null)
		{
			_components1 = registry.Components<T1>();
			_components2 = registry.Components<T2>();
			_include = include ?? Array.Empty<ISet>();
			_exclude = exclude ?? Array.Empty<ISet>();
			_componentsAndInclude = new ISet[_include.Length + 2];
			_componentsAndInclude[0] = _components1;
			_componentsAndInclude[1] = _components2;
			_include.CopyTo(_componentsAndInclude, 2);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void ForEach(EntityActionRef<T1, T2> action)
		{
			var data1 = _components1.AliveData;
			var data2 = _components2.AliveData;
			var ids = ViewUtils.GetMinimalSet(_componentsAndInclude).AliveIds;

			for (int i = ids.Length - 1; i >= 0; i--)
            {
            	int id = ids[i];
            	if (_components1.TryGetDense(id, out var dense1)
            	    && _components2.TryGetDense(id, out var dense2))
            	{
            		if (ViewUtils.AliveInAll(id, _include) && ViewUtils.NotAliveInAll(id, _exclude))
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
			var ids = ViewUtils.GetMinimalSet(_componentsAndInclude).AliveIds;

			for (int i = ids.Length - 1; i >= 0; i--)
			{
				int id = ids[i];
				if (_components1.TryGetDense(id, out var dense1)
				    && _components2.TryGetDense(id, out var dense2))
				{
					if (ViewUtils.AliveInAll(id, _include) && ViewUtils.NotAliveInAll(id, _exclude))
					{
						action.Invoke(id, ref data1[dense1], ref data2[dense2], extra);
					}
				}
			}
		}
	}
}