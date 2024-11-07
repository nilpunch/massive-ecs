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
		public ReactiveRegistry ReactiveRegistry { get; }
		public Entities Entities { get; }

		public int PageSize { get; }

		public Registry()
			: this(new RegistryConfig())
		{
		}

		public Registry(RegistryConfig registryConfig)
			: this(new NormalSetFactory(registryConfig.StoreEmptyTypesAsDataSets, registryConfig.PageSize, registryConfig.FullStability),
				new NormalReactiveFactory(), new Entities(), registryConfig.PageSize)
		{
		}

		protected Registry(ISetFactory setFactory, IReactiveFactory reactiveFactory, Entities entities, int pageSize)
		{
			SetRegistry = new SetRegistry(setFactory);
			ReactiveRegistry = new ReactiveRegistry(SetRegistry, reactiveFactory, entities);
			FilterRegistry = new FilterRegistry(SetRegistry);
			Entities = entities;
			PageSize = pageSize;

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
