using System;
using System.Runtime.CompilerServices;
using Unity.IL2CPP.CompilerServices;

namespace Massive
{
	[Il2CppSetOption(Option.NullChecks, false)]
	[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
	public class FilterView<T> where T : struct
	{
		private readonly Filter _filter;
		private readonly IDataSet<T> _components;
		private readonly ISet[] _componentsAndInclude;

		public FilterView(IRegistry registry, Filter filter)
		{
			_filter = filter;
			_components = registry.Components<T>();
			_componentsAndInclude = new ISet[_filter.Include.Length + 1];
			_componentsAndInclude[0] = _components;
			_filter.Include.CopyTo(_componentsAndInclude, 1);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void ForEach(EntityActionRef<T> action)
		{
			var data = _components.AliveData;
			var ids = SetUtils.GetMinimalSet(_componentsAndInclude).AliveIds;

			for (int i = ids.Length - 1; i >= 0; i--)
			{
				int id = ids[i];
				if (_components.TryGetDense(id, out var dense))
				{
					if (_filter.Contains(id))
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
			var ids = SetUtils.GetMinimalSet(_componentsAndInclude).AliveIds;

			for (int i = ids.Length - 1; i >= 0; i--)
			{
				int id = ids[i];
				if (_components.TryGetDense(id, out var dense))
				{
					if (_filter.Contains(id))
					{
						action.Invoke(id, ref data[dense], extra);
					}
				}
			}
		}
	}
}