namespace Massive.Samples.ECS
{
	[Unity.IL2CPP.CompilerServices.Il2CppSetOption(Unity.IL2CPP.CompilerServices.Option.NullChecks, false)]
	[Unity.IL2CPP.CompilerServices.Il2CppSetOption(Unity.IL2CPP.CompilerServices.Option.ArrayBoundsChecks, false)]
	[Unity.IL2CPP.CompilerServices.Il2CppSetOption(Unity.IL2CPP.CompilerServices.Option.DivideByZeroChecks, false)]
	public readonly struct View<T>
		where T : struct
	{
		private readonly MassiveDataSet<T> _components;

		public View(MassiveDataSet<T> components)
		{
			_components = components;
		}

		public void ForEach(EntityAction action) => ForEach((int id, ref T _) => action.Invoke(id));

		public void ForEach(ActionRef<T> action) => ForEach((int _, ref T value) => action.Invoke(ref value));

		public void ForEach(EntityActionRef<T> action)
		{
			var data = _components.AliveData;
			var ids = _components.AliveIds;
			for (int dense = ids.Length - 1; dense >= 0; dense--)
			{
				action.Invoke(ids[dense], ref data[dense]);
			}
		}
	}
}