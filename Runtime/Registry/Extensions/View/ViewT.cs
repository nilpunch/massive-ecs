using System.Runtime.CompilerServices;
using Unity.IL2CPP.CompilerServices;

namespace Massive.ECS
{
	[Il2CppSetOption(Option.NullChecks, false)]
	[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
	[Il2CppSetOption(Option.DivideByZeroChecks, false)]
	public readonly struct View<T> where T : unmanaged
	{
		private readonly IRegistry _registry;
		private readonly IDataSet<T> _components;

		public View(IRegistry registry)
		{
			_registry = registry;
			_components = registry.Component<T>();
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void ForEach(EntityAction action) => ForEach((Entity entity, ref T _) => action.Invoke(entity));

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void ForEach(ActionRef<T> action) => ForEach((Entity _, ref T value) => action.Invoke(ref value));

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void ForEach(EntityActionRef<T> action)
		{
			var data = _components.AliveData;
			var ids = _components.AliveIds;
			for (int dense = ids.Length - 1; dense >= 0; dense--)
			{
				action.Invoke(new Entity(_registry, ids[dense]), ref data[dense]);
			}
		}
	}
}