namespace Massive
{
	public class Registry : IRegistry
	{
		public SetRegistry SetRegistry { get; }
		public FilterRegistry FilterRegistry { get; }
		public GroupRegistry GroupRegistry { get; }
		public IEntities Entities { get; }

		public Registry(int setCapacity = Constants.DefaultSetCapacity, bool storeEmptyTypesAsDataSets = false, int pageSize = Constants.DefaultPageSize)
			: this(new NormalSetFactory(setCapacity, storeEmptyTypesAsDataSets, pageSize), new NormalGroupFactory(setCapacity), new Entities(setCapacity))
		{
		}

		protected Registry(ISetFactory setFactory, IGroupFactory groupFactory, IEntities entities)
		{
			SetRegistry = new SetRegistry(setFactory);
			GroupRegistry = new GroupRegistry(SetRegistry, groupFactory);
			FilterRegistry = new FilterRegistry(SetRegistry);
			Entities = entities;

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
