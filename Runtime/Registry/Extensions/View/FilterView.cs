using System.Runtime.CompilerServices;
using Unity.IL2CPP.CompilerServices;

namespace Massive.ECS
{
	[Il2CppSetOption(Option.NullChecks, false)]
	[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
	[Il2CppSetOption(Option.DivideByZeroChecks, false)]
	public readonly struct FilterView
	{
		private readonly IRegistry _registry;
		private readonly Filter _filter;
		private readonly ISet _entities;

		public FilterView(IRegistry registry, Filter filter)
		{
			_registry = registry;
			_filter = filter;
			_entities = _registry.Entities;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void ForEach(EntityAction action)
		{
			var ids = _entities.AliveIds;
			for (int dense = ids.Length - 1; dense >= 0; dense--)
			{
				int id = ids[dense];
				if (_filter.IsOkay(id))
				{
					action.Invoke(_registry.GetEntity(id));
				}
			}
		}
	}
}