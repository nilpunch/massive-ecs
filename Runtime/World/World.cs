using Unity.IL2CPP.CompilerServices;

namespace Massive
{
	[Il2CppSetOption(Option.NullChecks, false)]
	[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
	[Il2CppSetOption(Option.DivideByZeroChecks, false)]
	public class World
	{
		public Entities Entities { get; }

		public SetRegistry SetRegistry { get; }
		public FilterRegistry FilterRegistry { get; }
		public GroupRegistry GroupRegistry { get; }

		public WorldConfig Config { get; }

		public World()
			: this(new WorldConfig())
		{
		}

		public World(WorldConfig worldConfig)
			: this(new Entities(), new NormalSetFactory(worldConfig), new NormalGroupFactory(), worldConfig)
		{
		}

		public World(Entities entities, ISetFactory setFactory, IGroupFactory groupFactory, WorldConfig worldConfig)
		{
			Entities = entities;
			SetRegistry = new SetRegistry(setFactory);
			FilterRegistry = new FilterRegistry(SetRegistry);
			GroupRegistry = new GroupRegistry(SetRegistry, groupFactory, entities);
			Config = worldConfig;

			var allSets = SetRegistry.AllSets;
			Entities.BeforeDestroyed += RemoveFromAllSets;

			void RemoveFromAllSets(int entityId)
			{
				var setCount = allSets.Count;
				var sets = allSets.Items;
				for (var i = setCount - 1; i >= 0; i--)
				{
					sets[i].Remove(entityId);
				}
			}
		}
	}
}
