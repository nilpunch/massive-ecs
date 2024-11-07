using System.Runtime.CompilerServices;
using Unity.IL2CPP.CompilerServices;

namespace Massive
{
	[Il2CppSetOption(Option.NullChecks, false)]
	[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
	[Il2CppSetOption(Option.DivideByZeroChecks, false)]
	public static class RegistryCommonExtensions
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void Clear(this Registry registry)
		{
			registry.Entities.Clear();
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void Clear<T>(this Registry registry)
		{
			registry.Set<T>().Clear();
		}
	}
}
