using System.Runtime.CompilerServices;
using Unity.IL2CPP.CompilerServices;

namespace Massive
{
	[Il2CppSetOption(Option.NullChecks, false)]
	[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
	public static class WorldViewExtensions
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static View View(this World world)
		{
			return new View(world, world.Config.PackingWhenIterating);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static View View(this World world, Packing packingWhenIterating)
		{
			return new View(world, packingWhenIterating);
		}
	}
}
