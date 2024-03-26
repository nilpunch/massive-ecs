using System.Runtime.CompilerServices;
using Unity.IL2CPP.CompilerServices;

namespace Massive
{
	[Il2CppSetOption(Option.NullChecks, false)]
	[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
	public class FilterView
	{
		private readonly IFilter _filter;
		private readonly Identifiers _entities;

		public FilterView(IRegistry registry, IFilter filter = null)
		{
			_filter = filter ?? EmptyFilter.Instance;
			_entities = registry.Entities;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void ForEach(EntityAction action)
		{
			var ids = _filter.Include.Length == 0
				? _entities.AliveIds
				: SetUtils.GetMinimalSet(_filter.Include).AliveIds;

			for (var i = ids.Length - 1; i >= 0; i--)
			{
				var id = ids[i];
				if (_filter.Contains(id))
				{
					action.Invoke(id);
				}
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void ForEachExtra<TExtra>(TExtra extra, EntityActionExtra<TExtra> action)
		{
			var ids = _filter.Include.Length == 0
				? _entities.AliveIds
				: SetUtils.GetMinimalSet(_filter.Include).AliveIds;

			for (var i = ids.Length - 1; i >= 0; i--)
			{
				var id = ids[i];
				if (_filter.Contains(id))
				{
					action.Invoke(id, extra);
				}
			}
		}
	}
}