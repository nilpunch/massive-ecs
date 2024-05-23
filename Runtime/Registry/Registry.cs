using System.Collections.Generic;

namespace Massive
{
	public class Registry : IRegistry
	{
		protected IReadOnlyList<ISet> AllSets => SetCollection.AllSets;

		public IndexedSetCollection SetCollection { get; }
		public IGroupsController Groups { get; }
		public IEntities Entities { get; }

		public Registry(int setCapacity = Constants.DefaultSetCapacity, bool storeEmptyTypesAsDataSets = false, int pageSize = Constants.DefaultPageSize)
			: this(new GroupsController(setCapacity), new Entities(setCapacity), new NormalSetFactory(setCapacity, storeEmptyTypesAsDataSets, pageSize))
		{
		}

		protected Registry(IGroupsController groups, IEntities entities, ISetFactory setFactory)
		{
			Groups = groups;
			Entities = entities;
			SetCollection = new IndexedSetCollection(setFactory);

			Entities.BeforeDestroyed += UnassignFromAllSets;
		}

		private void UnassignFromAllSets(int id)
		{
			for (var i = 0; i < SetCollection.AllSets.Count; i++)
			{
				SetCollection.AllSets[i].Unassign(id);
			}
		}
	}
}
