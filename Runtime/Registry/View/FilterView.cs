using System.Runtime.CompilerServices;
using Unity.IL2CPP.CompilerServices;

namespace Massive
{
	[Il2CppSetOption(Option.NullChecks, false)]
	[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
	public readonly struct FilterView
	{
		private readonly IFilter _filter;
		private readonly Entities _entities;

		public FilterView(IRegistry registry, IFilter filter = null)
		{
			_filter = filter ?? EmptyFilter.Instance;
			_entities = registry.Entities;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void ForEach(EntityAction action)
		{
			if (_filter.Include.Length == 0)
			{
				var entities = _entities.Alive;

				for (var i = entities.Length - 1; i >= 0; i--)
				{
					var entity = entities[i];
					if (_filter.ContainsId(entity.Id))
					{
						action.Invoke(entity.Id);
					}
				}
			}
			else
			{
				var ids = SetHelpers.GetMinimalSet(_filter.Include).Ids;

				for (var i = ids.Length - 1; i >= 0; i--)
				{
					var id = ids[i];
					if (_filter.ContainsId(id))
					{
						action.Invoke(id);
					}
				}
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void ForEachExtra<TExtra>(TExtra extra, EntityActionExtra<TExtra> action)
		{
			if (_filter.Include.Length == 0)
			{
				var entities = _entities.Alive;

				for (var i = entities.Length - 1; i >= 0; i--)
				{
					var entity = entities[i];
					if (_filter.ContainsId(entity.Id))
					{
						action.Invoke(entity.Id, extra);
					}
				}
			}
			else
			{
				var ids = SetHelpers.GetMinimalSet(_filter.Include).Ids;

				for (var i = ids.Length - 1; i >= 0; i--)
				{
					var id = ids[i];
					if (_filter.ContainsId(id))
					{
						action.Invoke(id, extra);
					}
				}
			}
		}
	}
}