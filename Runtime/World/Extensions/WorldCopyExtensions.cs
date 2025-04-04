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
			var srcRegistry = source.SetRegistry;
			var dstRegistry = destination.SetRegistry;

			foreach (var cloner in srcRegistry.Cloners)
			{
				cloner.CopyTo(dstRegistry);
			}

			// Clear other sets.
			var dstSets = dstRegistry.AllSets;
			var srcHashes = srcRegistry.Hashes;
			var dstHashes = srcRegistry.Hashes;

			var src = 0;
			for (var dst = 0; dst < dstSets.Count; dst++)
			{
				if (src >= srcHashes.Count || dstHashes[dst] != srcHashes[src])
				{
					dstSets[dst].Clear();
				}
				else
				{
					src++;
				}
			}
		}
	}
}
