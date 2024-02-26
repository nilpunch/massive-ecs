using System.Runtime.CompilerServices;

namespace Massive.ECS
{
	[Unity.IL2CPP.CompilerServices.Il2CppSetOption(Unity.IL2CPP.CompilerServices.Option.NullChecks, false)]
	[Unity.IL2CPP.CompilerServices.Il2CppSetOption(Unity.IL2CPP.CompilerServices.Option.ArrayBoundsChecks, false)]
	[Unity.IL2CPP.CompilerServices.Il2CppSetOption(Unity.IL2CPP.CompilerServices.Option.DivideByZeroChecks, false)]
	public readonly struct View<T1, T2>
		where T1 : unmanaged
		where T2 : unmanaged
	{
		private readonly IRegistry _registry;
		private readonly IDataSet<T1> _components1;
		private readonly IDataSet<T2> _components2;

		public View(IRegistry registry)
		{
			_registry = registry;
			_components1 = _registry.Component<T1>();
			_components2 = _registry.Component<T2>();
		}
		
		public DoubleViewEnumerator GetEnumerator()
		{
			return new DoubleViewEnumerator(_registry, _components1, _components2);
		}
	}
}