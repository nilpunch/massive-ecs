#if !MASSIVE_DISABLE_ASSERT
#define MASSIVE_ASSERT
#endif

using System.Runtime.CompilerServices;
using Unity.IL2CPP.CompilerServices;

namespace Massive
{
	[Il2CppSetOption(Option.NullChecks, false)]
	[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
	[Il2CppSetOption(Option.DivideByZeroChecks, false)]
	public static class WorldCopyExtensions
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static World Clone(this World world)
		{
			var clone = new World(world.Config);
			world.CopyTo(clone);
			return clone;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void CopyTo(this World source, World destination)
		{
			Assert.CompatibleConfigs(source, destination);

			source.Entities.CopyTo(destination.Entities);

			// To be continued...
		}
	}
}
