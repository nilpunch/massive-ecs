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
		/// <summary>
		/// Creates and returns a new world that is an exact copy of the current one.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static World Clone(this World world)
		{
			var clone = new World(world.Config);
			world.CopyTo(clone);
			return clone;
		}

		/// <summary>
		/// Copies the contents of the current world into the specified one.
		/// Clears sets in the target world that are not present in the source.
		/// </summary>
		/// <remarks>
		/// Throws an exception if the worlds have incompatible configs.
		/// </remarks>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void CopyTo(this World world, World other)
		{
			Assert.CompatibleConfigs(world, other);

			// Entities.
			world.Entities.CopyTo(other.Entities);

			// Sets.
			var registry = world.SetRegistry;
			var otherRegistry = other.SetRegistry;

			foreach (var cloner in registry.Cloners)
			{
				cloner.CopyTo(otherRegistry);
			}

			// Clear other sets.
			var hashes = registry.Hashes;
			var otherHashes = registry.Hashes;
			var otherSets = otherRegistry.AllSets;

			var index = 0;
			for (var otherIndex = 0; otherIndex < otherSets.Count; otherIndex++)
			{
				if (index >= hashes.Count || otherHashes[otherIndex] != hashes[index])
				{
					otherSets[otherIndex].ClearWithoutNotify();
				}
				else
				{
					index++;
				}
			}
		}
	}
}
