namespace Massive.Samples.ECS
{
	[Unity.IL2CPP.CompilerServices.Il2CppSetOption(Unity.IL2CPP.CompilerServices.Option.NullChecks, false)]
	[Unity.IL2CPP.CompilerServices.Il2CppSetOption(Unity.IL2CPP.CompilerServices.Option.ArrayBoundsChecks, false)]
	[Unity.IL2CPP.CompilerServices.Il2CppSetOption(Unity.IL2CPP.CompilerServices.Option.DivideByZeroChecks, false)]
	public class View<T>
		where T : struct
	{
		private readonly MassiveDataSet<T> _components;
		private readonly Filter _filter;

		public View(MassiveDataSet<T> components, Filter filter = null)
		{
			_components = components;
			_filter = filter;
		}

		public void ForEach(EntityActionRef<T> action)
		{
			if (_filter is null)
			{
				ForEachRaw(action);
			}
			else
			{
				ForEachFiltered(action);
			}
		}

		private void ForEachRaw(EntityActionRef<T> action)
		{
			var data = _components.AliveData;
			var ids = _components.AliveIds;
			for (int dense = ids.Length - 1; dense >= 0; dense--)
			{
				action.Invoke(ids[dense], ref data[dense]);
			}
		}

		private void ForEachFiltered(EntityActionRef<T> action)
		{
			var data = _components.AliveData;
			var ids = _components.AliveIds;
			for (int dense = ids.Length - 1; dense >= 0; dense--)
			{
				int id = ids[dense];
				if (_filter.IsOkay(id))
				{
					action.Invoke(id, ref data[dense]);
				}
			}
		}
	}
}