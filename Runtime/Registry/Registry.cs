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

			var allSets = SetRegistry.All;
			Entities.BeforeDestroyed += UnassignFromAllSets;

			void UnassignFromAllSets(int entityId)
			{
				var setCount = allSets.Count;
				var sets = allSets.Items;
				for (var i = setCount - 1; i >= 0; i--)
				{
					sets[i].Unassign(entityId);
				}
			}
		}
	}
}
