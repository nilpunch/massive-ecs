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
			var sourceSetRegistry = source.SetRegistry;
			var destinationSetRegistry = destination.SetRegistry;

			foreach (var cloner in sourceSetRegistry.Cloners)
			{
				cloner.CopyTo(destinationSetRegistry);
			}

			// Clear other sets.
			var destinationSets = destinationSetRegistry.AllSets;
			var sourceSetHashes = sourceSetRegistry.Hashes;
			var destinationSetHashes = sourceSetRegistry.Hashes;

			int src = 0;
			for (int dst = 0; dst < destinationSets.Count; dst++)
			{
				if (src >= sourceSetHashes.Count || destinationSetHashes[dst] != sourceSetHashes[src])
				{
					destinationSets[dst].Clear();
				}
				else
				{
					src++;
				}
			}
		}
	}
}
