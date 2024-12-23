using Unity.IL2CPP.CompilerServices;

namespace Massive
{
	[Il2CppSetOption(Option.NullChecks, false)]
	[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
	[Il2CppSetOption(Option.DivideByZeroChecks, false)]
	public class Registry
	{
		public Entities Entities { get; }

		public SetRegistry SetRegistry { get; }
		public FilterRegistry FilterRegistry { get; }
		public GroupRegistry GroupRegistry { get; }
		public ServiceRegistry ServiceRegistry { get; }

		public RegistryConfig Config { get; }

		public Registry()
			: this(new RegistryConfig())
		{
		}

		public Registry(RegistryConfig registryConfig)
			: this(new Entities(), new NormalSetFactory(registryConfig), new NormalGroupFactory(), registryConfig)
		{
		}

		protected Registry(Entities entities, ISetFactory setFactory, IGroupFactory groupFactory, RegistryConfig registryConfig)
		{
			Entities = entities;
			SetRegistry = new SetRegistry(setFactory);
			FilterRegistry = new FilterRegistry(SetRegistry);
			GroupRegistry = new GroupRegistry(SetRegistry, groupFactory, entities);
			ServiceRegistry = new ServiceRegistry();
			Config = registryConfig;

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
