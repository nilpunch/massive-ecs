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
		public Entities Entities { get; }

		public Sets Sets { get; }
		public Components Components { get; }
		public Filters Filters { get; }
		public Allocators Allocators { get; }

		public WorldConfig Config { get; }

		public World()
			: this(new WorldConfig())
		{
		}

		public World(WorldConfig worldConfig)
		{
			Components = new Components();
			Sets = new Sets(new SetFactory(worldConfig), Components);
			Filters = new Filters(Sets);
			Allocators = new Allocators();
			Config = worldConfig;
			Entities = new Entities(this);
		}
	}
}
