#if !MASSIVE_DISABLE_ASSERT
#define MASSIVE_ASSERT
#endif

using System.Runtime.CompilerServices;
using Unity.IL2CPP.CompilerServices;

namespace Massive
{
	[Il2CppSetOption(Option.NullChecks, false)]
	[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
	public partial class World
	{
		public Entities Entities { get; }
		public Components Components { get; }

		public Sets Sets { get; }
		public Allocator Allocator { get; }

		public WorldConfig Config { get; }

		public World()
			: this(new WorldConfig())
		{
		}

		public World(WorldConfig worldConfig)
		{
			Allocator = new Allocator();
			Components = new Components();
			Sets = new Sets(new SetFactory(Allocator, worldConfig), Components);
			Config = worldConfig;
			Entities = new Entities(this);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static implicit operator Allocator(World world)
		{
			return world.Allocator;
		}
	}
}
