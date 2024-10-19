using Unity.IL2CPP.CompilerServices;

namespace Massive
{
	[Il2CppSetOption(Option.NullChecks | Option.ArrayBoundsChecks, false)]
	public class Registry
	{
		public SetRegistry SetRegistry { get; }
		public FilterRegistry FilterRegistry { get; }
		public GroupRegistry GroupRegistry { get; }
		public Entities Entities { get; }

		public Registry()
			: this(new RegistryConfig())
		{
		}

		public Registry(RegistryConfig registryConfig)
			: this(new NormalSetFactory(registryConfig.SetCapacity, registryConfig.StoreEmptyTypesAsDataSets, registryConfig.DataPageSize, registryConfig.FullStability),
				new NormalGroupFactory(registryConfig.SetCapacity), new Entities(registryConfig.SetCapacity))
		{
		}

		protected Registry(ISetFactory setFactory, IGroupFactory groupFactory, Entities entities)
		{
			SetRegistry = new SetRegistry(setFactory);
			GroupRegistry = new GroupRegistry(SetRegistry, groupFactory);
			FilterRegistry = new FilterRegistry(SetRegistry);
			Entities = entities;

			Entities.BeforeDestroyed += UnassignFromAllSets;
		}

		private void UnassignFromAllSets(int entityId)
		{
			var sets = SetRegistry.All;
			for (var i = 0; i < sets.Length; i++)
			{
				sets[i].Unassign(entityId);
			}
		}
	}
}
