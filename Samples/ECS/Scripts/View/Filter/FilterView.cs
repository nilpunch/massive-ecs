namespace Massive.Samples.ECS
{
	[Unity.IL2CPP.CompilerServices.Il2CppSetOption(Unity.IL2CPP.CompilerServices.Option.NullChecks, false)]
	[Unity.IL2CPP.CompilerServices.Il2CppSetOption(Unity.IL2CPP.CompilerServices.Option.ArrayBoundsChecks, false)]
	[Unity.IL2CPP.CompilerServices.Il2CppSetOption(Unity.IL2CPP.CompilerServices.Option.DivideByZeroChecks, false)]
	public class FilterView : IView
	{
		private readonly MassiveSparseSet _tags;
		private readonly Filter _filter;

		public FilterView(MassiveSparseSet tags, Filter filter)
		{
			_tags = tags;
			_filter = filter;
		}

		public void ForEach(EntityAction action)
		{
			var ids = _tags.AliveIds;
			for (int dense = ids.Length - 1; dense >= 0; dense--)
			{
				int id = ids[dense];
				if (_filter.IsOkay(id))
				{
					action.Invoke(id);
				}
			}
		}
	}
}