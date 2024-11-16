using Unity.IL2CPP.CompilerServices;

namespace Massive
{
	[Il2CppSetOption(Option.NullChecks, false)]
	[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
	[Il2CppSetOption(Option.DivideByZeroChecks, false)]
	public class Registry
	{
		public SetRegistry SetRegistry { get; }
		public FilterRegistry FilterRegistry { get; }
		public GroupRegistry GroupRegistry { get; }
		public Entities Entities { get; }

		public int PageSize { get; }

		public Registry()
			: this(new RegistryConfig())
		{
		}

		public Registry(RegistryConfig registryConfig)
			: this(new NormalSetFactory(registryConfig.StoreEmptyTypesAsDataSets, registryConfig.PageSize, registryConfig.FullStability),
				new NormalGroupFactory(), new Entities(), registryConfig.PageSize)
		{
		}

		protected Registry(ISetFactory setFactory, IGroupFactory groupFactory, Entities entities, int pageSize)
		{
			SetRegistry = new SetRegistry(setFactory);
			GroupRegistry = new GroupRegistry(SetRegistry, groupFactory, entities);
			FilterRegistry = new FilterRegistry(SetRegistry);
			Entities = entities;
			PageSize = pageSize;

			Entities.BeforeDestroyed += UnassignFromAllSets;
		}

		private void UnassignFromAllSets(int entityId)
		{
			var setList = SetRegistry.All;
			var setCount = setList.Count;
			var sets = setList.Items;
			for (var i = 0; i < setCount; i++)
			{
				sets[i].Unassign(entityId);
			}
		}
	}
}
