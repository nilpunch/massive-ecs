namespace Massive
{
	public class Registry : IRegistry
	{
		public SetRegistry SetRegistry { get; }
		public FilterRegistry FilterRegistry { get; }
		public GroupRegistry GroupRegistry { get; }
		public IEntities Entities { get; }

		public Registry(int setCapacity = Constants.DefaultSetCapacity, bool storeEmptyTypesAsDataSets = false, int pageSize = Constants.DefaultPageSize)
			: this(new GroupRegistry(setCapacity), new Entities(setCapacity), new NormalSetFactory(setCapacity, storeEmptyTypesAsDataSets, pageSize))
		{
		}

		protected Registry(GroupRegistry groupRegistry, IEntities entities, ISetFactory setFactory)
		{
			GroupRegistry = groupRegistry;
			Entities = entities;
			SetRegistry = new SetRegistry(setFactory);
			FilterRegistry = new FilterRegistry();

			Entities.BeforeDestroyed += UnassignFromAllSets;
		}

		private void UnassignFromAllSets(int id)
		{
			for (var i = 0; i < SetRegistry.All.Count; i++)
			{
				SetRegistry.All[i].Unassign(id);
			}
		}
	}
}
