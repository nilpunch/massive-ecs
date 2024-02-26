using System.Runtime.CompilerServices;

namespace Massive.ECS
{
	[Unity.IL2CPP.CompilerServices.Il2CppSetOption(Unity.IL2CPP.CompilerServices.Option.NullChecks, false)]
	[Unity.IL2CPP.CompilerServices.Il2CppSetOption(Unity.IL2CPP.CompilerServices.Option.ArrayBoundsChecks, false)]
	[Unity.IL2CPP.CompilerServices.Il2CppSetOption(Unity.IL2CPP.CompilerServices.Option.DivideByZeroChecks, false)]
	public readonly struct View<T> where T : unmanaged
	{
		private readonly IRegistry _registry;
		private readonly IDataSet<T> _components;

		public View(IRegistry registry)
		{
			_registry = registry;
			_components = registry.Component<T>();
		}
		
		public ViewEnumerator GetEnumerator()
		{
			return new ViewEnumerator(_registry, _components.AliveIds);
		}
	}
}