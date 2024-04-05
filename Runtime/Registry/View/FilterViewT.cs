using System.Linq;
using System.Runtime.CompilerServices;
using Unity.IL2CPP.CompilerServices;

namespace Massive
{
	[Il2CppSetOption(Option.NullChecks, false)]
	[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
	public class FilterView<T> where T : struct
	{
		private readonly IFilter _filter;
		private readonly IReadOnlyDataSet<T> _components;
		private readonly IReadOnlySet[] _componentsAndInclude;

		public FilterView(IRegistry registry, IFilter filter = null)
		{
			_filter = filter ?? EmptyFilter.Instance;
			_components = registry.Components<T>();
			_componentsAndInclude = Enumerable.Empty<IReadOnlySet>()
				.Append(_components)
				.Concat(_filter.Include)
				.ToArray();
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void ForEach(EntityActionRef<T> action)
		{
			var data = _components.AliveData;
			var ids = SetHelpers.GetMinimalSet(_componentsAndInclude).AliveIds;

			for (int i = ids.Length - 1; i >= 0; i--)
			{
				int id = ids[i];
				if (_components.TryGetDense(id, out var dense))
				{
					if (_filter.ContainsId(id))
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
			var ids = SetHelpers.GetMinimalSet(_componentsAndInclude).AliveIds;

			for (int i = ids.Length - 1; i >= 0; i--)
			{
				int id = ids[i];
				if (_components.TryGetDense(id, out var dense))
				{
					if (_filter.ContainsId(id))
					{
						action.Invoke(id, ref data[dense], extra);
					}
				}
			}
		}
	}
}