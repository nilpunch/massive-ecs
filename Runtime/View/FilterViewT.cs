using System.Runtime.CompilerServices;
using Unity.IL2CPP.CompilerServices;

namespace Massive
{
	[Il2CppSetOption(Option.NullChecks, false)]
	[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
	public readonly struct FilterView<T>
	{
		private readonly IFilter _filter;
		private readonly IReadOnlyDataSet<T> _components;

		public FilterView(IRegistry registry, IFilter filter = null)
		{
			_filter = filter ?? EmptyFilter.Instance;
			_components = registry.Components<T>();
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void ForEach(EntityActionRef<T> action)
		{
			var data = _components.Data;
			var ids = SetHelpers.GetMinimalSet(_components, _filter.Include).Ids;

			for (int i = ids.Length - 1; i >= 0; i--)
			{
				var id = ids[i];
				if (_components.TryGetDense(id, out var dense)
				    && _filter.ContainsId(id))
				{
					action.Invoke(id, ref data[dense]);
				}
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void ForEachExtra<TExtra>(TExtra extra, EntityActionRefExtra<T, TExtra> action)
		{
			var data = _components.Data;
			var ids = SetHelpers.GetMinimalSet(_components, _filter.Include).Ids;

			for (int i = ids.Length - 1; i >= 0; i--)
			{
				var id = ids[i];
				if (_components.TryGetDense(id, out var dense)
				    && _filter.ContainsId(id))
				{
					action.Invoke(id, ref data[dense], extra);
				}
			}
		}
	}
}
