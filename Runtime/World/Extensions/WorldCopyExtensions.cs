#if !MASSIVE_DISABLE_ASSERT
#define MASSIVE_ASSERT
#endif

using System.Runtime.CompilerServices;
using Unity.IL2CPP.CompilerServices;

namespace Massive
{
	[Il2CppSetOption(Option.NullChecks, false)]
	[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
	public static class WorldCopyExtensions
	{
		/// <summary>
		/// Creates and returns a new world that is an exact copy of this one.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static World Clone(this World world)
		{
			var clone = new World(world.Config);
			world.CopyTo(clone);
			return clone;
		}

		/// <summary>
		/// Copies the contents of this world into the specified one.
		/// Clears sets in the target world that are not present in the source.
		/// </summary>
		/// <remarks>
		/// Throws if the worlds have incompatible configs.
		/// </remarks>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void CopyTo(this World world, World other)
		{
			MassiveAssert.CompatibleConfigs(world, other);

			// Entities.
			world.Entities.CopyTo(other.Entities);

			// Sets.
			world.SetRegistry.CopyTo(other.SetRegistry);
		}
	}
}
