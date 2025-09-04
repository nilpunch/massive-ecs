#if !MASSIVE_DISABLE_ASSERT
#define MASSIVE_ASSERT
#endif

using Unity.IL2CPP.CompilerServices;

namespace Massive
{
	[Il2CppSetOption(Option.NullChecks, false)]
	[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
	public partial class World
	{
		public Entifiers Entifiers { get; }

		public BitSets BitSets { get; }
		public Masks Masks { get; }
		public Filters Filters { get; }
		public Allocators Allocators { get; }

		public WorldConfig Config { get; }

		public World()
			: this(new WorldConfig())
		{
		}

		public World(WorldConfig worldConfig)
		{
			Masks = new Masks();
			Entifiers = new Entifiers();
			BitSets = new BitSets(new SetFactory(worldConfig));
			Filters = new Filters(BitSets, Masks, worldConfig.OptimizeExludeFilter);
			Allocators = new Allocators();
			Config = worldConfig;

			BitSets.Entifiers = Entifiers;
			BitSets.Masks = Masks;
			Entifiers.WorldContext = new WorldContext(BitSets, Allocators, Masks);
		}
	}
}
