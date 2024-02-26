using System.Runtime.CompilerServices;
using Unity.IL2CPP.CompilerServices;

namespace Massive.ECS
{
	[Il2CppSetOption(Option.NullChecks, false)]
	[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
	[Il2CppSetOption(Option.DivideByZeroChecks, false)]
	public readonly struct View
	{
		private readonly IRegistry _registry;
		private readonly ISet _entities;

		public View(IRegistry registry)
		{
			_registry = registry;
			_entities = registry.Entities;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void ForEach(EntityAction action)
		{
			var ids = _entities.AliveIds;
			for (int dense = ids.Length - 1; dense >= 0; dense--)
			{
				action.Invoke(_registry.GetEntity(ids[dense]));
			}
		}
	}
}