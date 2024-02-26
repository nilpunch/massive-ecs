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

		public ViewEnumerator GetEnumerator()
		{
			return new ViewEnumerator(_registry, _entities.AliveIds);
		}
	}
}