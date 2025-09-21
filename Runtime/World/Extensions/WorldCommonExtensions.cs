using System.Runtime.CompilerServices;
using Unity.IL2CPP.CompilerServices;

namespace Massive
{
	[Il2CppEagerStaticClassConstruction]
	[Il2CppSetOption(Option.NullChecks, false)]
	[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
	public static class WorldCommonExtensions
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void Clear(this World world)
		{
			world.Entities.Clear();
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void Clear<T>(this World world)
		{
			world.BitSet<T>().Clear();
		}
	}
}
