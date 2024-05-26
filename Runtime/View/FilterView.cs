using System.Runtime.CompilerServices;
using Unity.IL2CPP.CompilerServices;

namespace Massive
{
	[Il2CppSetOption(Option.NullChecks, false)]
	[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
	public readonly struct FilterView : IView
	{
		private readonly IFilter _filter;
		private readonly IEntities _entities;

		public FilterView(IRegistry registry, IFilter filter = null)
		{
			_filter = filter ?? EmptyFilter.Instance;
			_entities = registry.Entities;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void ForEachUniversal<TInvoker>(TInvoker invoker)
			where TInvoker : IEntityActionInvoker
		{
			if (_filter.Include.Count == 0)
			{
				var entities = _entities.Alive;

				for (var i = entities.Length - 1; i >= 0; i--)
				{
					var entity = entities[i];
					if (_filter.ContainsId(entity.Id))
					{
						invoker.Apply(entity.Id);
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
						invoker.Apply(id);
					}
				}
			}
		}
	}
}
