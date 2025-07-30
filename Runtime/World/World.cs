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
			var allNegativeSets = Sets.AllNegativeSets;
			var allocators = Allocators;
			Entities.BeforeDestroyed += RemoveFromAll;
			Entities.AfterCreated += AddToNegative;
			Sets.SetPairCreated += PairSets;

			void RemoveFromAll(int entityId)
			{
				var setCount = allSets.Count;
				var sets = allSets.Items;
				for (var i = setCount - 1; i >= 0; i--)
				{
					var negative = sets[i].Negative;
					sets[i].Negative = null; // Don't propagate change to negative sets.
					sets[i].Remove(entityId);
					sets[i].Negative = negative;
				}

				allocators.Free(entityId);
			}

			void AddToNegative(int entityId)
			{
				var setCount = allNegativeSets.Count;
				var sets = allNegativeSets.Items;
				for (var i = setCount - 1; i >= 0; i--)
				{
					sets[i].Add(entityId);
				}
			}

			void PairSets((SparseSet Positive, SparseSet Negative) pair)
			{
				foreach (var entity in Entities)
				{
					if (!pair.Positive.Has(entity))
					{
						pair.Negative.Add(entity);
					}
				}

				pair.Positive.Negative = pair.Negative;
				pair.Negative.Negative = pair.Positive;
			}
		}
	}
}
