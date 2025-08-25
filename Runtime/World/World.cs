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

		public Sets Sets { get; }
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
			Sets = new Sets(new SetFactory(worldConfig));
			Filters = new Filters(Sets, Masks, worldConfig.OptimizeExludeFilter);
			Allocators = new Allocators();
			Config = worldConfig;

			Sets.Entifiers = Entifiers;
			Sets.Masks = Masks;
			Entifiers.WorldContext = new WorldContext(Sets, Allocators, Masks);
		}
	}
}
