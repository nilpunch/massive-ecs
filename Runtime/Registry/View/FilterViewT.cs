using System;
using System.Runtime.CompilerServices;
using Unity.IL2CPP.CompilerServices;

namespace Massive
{
	[Il2CppSetOption(Option.NullChecks, false)]
	[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
	public class FilterView<T> where T : struct
	{
		private readonly IDataSet<T> _components;
		private readonly ISet[] _exclude;
		private readonly ISet[] _include;
		private readonly ISet[] _componentsAndInclude;

		public FilterView(IRegistry registry, ISet[] include = null, ISet[] exclude = null)
		{
			_components = registry.Components<T>();
			_include = include ?? Array.Empty<ISet>();
			_exclude = exclude ?? Array.Empty<ISet>();
			_componentsAndInclude = new ISet[_include.Length + 1];
			_componentsAndInclude[0] = _components;
			_include.CopyTo(_componentsAndInclude, 1);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void ForEach(EntityActionRef<T> action)
		{
			var data = _components.AliveData;
			var ids = ViewUtils.GetMinimalSet(_componentsAndInclude).AliveIds;

			for (int i = ids.Length - 1; i >= 0; i--)
            {
            	int id = ids[i];
            	if (_components.TryGetDense(id, out var dense))
            	{
            		if (ViewUtils.AliveInAll(id, _include) && ViewUtils.NotAliveInAll(id, _exclude))
            		{
            			action.Invoke(id, ref data[dense]);
            		}
            	}
            }
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void ForEachExtra<TExtra>(TExtra extra, EntityActionRefExtra<T, TExtra> action)
		{
			var data = _components.AliveData;
			var ids = ViewUtils.GetMinimalSet(_componentsAndInclude).AliveIds;

			for (int i = ids.Length - 1; i >= 0; i--)
			{
				int id = ids[i];
				if (_components.TryGetDense(id, out var dense))
				{
					if (ViewUtils.AliveInAll(id, _include) && ViewUtils.NotAliveInAll(id, _exclude))
					{
						action.Invoke(id, ref data[dense], extra);
					}
				}
			}
		}
	}
}