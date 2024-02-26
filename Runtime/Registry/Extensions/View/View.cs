using System.Runtime.CompilerServices;

namespace Massive.ECS
{
	[Unity.IL2CPP.CompilerServices.Il2CppSetOption(Unity.IL2CPP.CompilerServices.Option.NullChecks, false)]
	[Unity.IL2CPP.CompilerServices.Il2CppSetOption(Unity.IL2CPP.CompilerServices.Option.ArrayBoundsChecks, false)]
	[Unity.IL2CPP.CompilerServices.Il2CppSetOption(Unity.IL2CPP.CompilerServices.Option.DivideByZeroChecks, false)]
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