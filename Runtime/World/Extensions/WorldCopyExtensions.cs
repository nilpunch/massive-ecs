#if !MASSIVE_DISABLE_ASSERT
#define MASSIVE_ASSERT
#endif

using Unity.IL2CPP.CompilerServices;

namespace Massive
{
	[Il2CppEagerStaticClassConstruction]
	[Il2CppSetOption(Option.NullChecks, false)]
	[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
	public static class WorldCopyExtensions
	{
		/// <summary>
		/// Creates and returns a new world that is an exact copy of this one.
		/// </summary>
		public static World Clone(this World world)
		{
			var clone = new World(world.Config);
			world.CopyTo(clone);
			return clone;
		}

		/// <summary>
		/// Copies the contents of this world into the specified one.<br/>
		/// Clears sets in the target world that are not present in the source.<br/>
		/// Resets allocators in the target world that are not present in the source.
		/// </summary>
		/// <remarks>
		/// Throws if the worlds have incompatible configs.
		/// </remarks>
		public static void CopyTo(this World world, World other)
		{
			IncompatibleConfigsException.ThrowIfIncompatible(world, other);

			world.Entities.CopyTo(other.Entities);
			world.Components.CopyTo(other.Components);
			world.Sets.CopyTo(other.Sets);
			world.Allocator.CopyTo(other.Allocator);
		}
	}
}
