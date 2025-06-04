using Unity.IL2CPP.CompilerServices;

namespace Massive
{
	[Il2CppSetOption(Option.NullChecks, false)]
	[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
	public partial class World
	{
		public Entities Entities { get; }

		public Sets Sets { get; }
		public Filters Filters { get; }
		public Allocators Allocators { get; }

		public WorldConfig Config { get; }

		public World()
			: this(new WorldConfig())
		{
		}

		public World(WorldConfig worldConfig)
		{
			Entities = new Entities();
			Sets = new Sets(new SetFactory(worldConfig));
			Filters = new Filters(Sets);
			Allocators = new Allocators();
			Config = worldConfig;

			var allSets = Sets.AllSets;
			var allocators = Allocators;
			Entities.BeforeDestroyed += RemoveFromAll;

			void RemoveFromAll(int entityId)
			{
				var setCount = allSets.Count;
				var sets = allSets.Items;
				for (var i = setCount - 1; i >= 0; i--)
				{
					sets[i].Remove(entityId);
				}

				allocators.Free(entityId);
			}
		}
	}
}
