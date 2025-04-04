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

			// Entities.
			source.Entities.CopyTo(destination.Entities);

			// Sets.
			var destinationSets = destination.SetRegistry;

			var clonersList = source.SetRegistry.Cloners;
			var clonersCount = clonersList.Count;
			var cloners = clonersList.Items;
			for (var i = 0; i < clonersCount; i++)
			{
				cloners[i].CopyTo(destinationSets);
			}
		}
	}
}
