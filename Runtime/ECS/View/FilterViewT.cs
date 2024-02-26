using System.Runtime.CompilerServices;

namespace Massive.ECS
{
	[Unity.IL2CPP.CompilerServices.Il2CppSetOption(Unity.IL2CPP.CompilerServices.Option.NullChecks, false)]
	[Unity.IL2CPP.CompilerServices.Il2CppSetOption(Unity.IL2CPP.CompilerServices.Option.ArrayBoundsChecks, false)]
	[Unity.IL2CPP.CompilerServices.Il2CppSetOption(Unity.IL2CPP.CompilerServices.Option.DivideByZeroChecks, false)]
	public readonly struct FilterView<T> where T : unmanaged
	{
		private readonly Registry _registry;
		private readonly Filter _filter;
		private readonly IDataSet<T> _components;

		public FilterView(Registry registry, Filter filter)
		{
			_registry = registry;
			_filter = filter;
			_components = registry.Component<T>();
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void ForEach(EntityAction action) => ForEach((Entity id, ref T _) => action.Invoke(id));

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void ForEach(ActionRef<T> action) => ForEach((Entity _, ref T value) => action.Invoke(ref value));

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void ForEach(EntityActionRef<T> action)
		{
			var data = _components.AliveData;
			var ids = _components.AliveIds;
			for (int dense = ids.Length - 1; dense >= 0; dense--)
			{
				int id = ids[dense];
				if (_filter.IsOkay(id))
				{
					action.Invoke(_registry.GetEntity(id), ref data[dense]);
				}
			}
		}
	}
}