using System.Runtime.CompilerServices;
using Unity.IL2CPP.CompilerServices;

namespace Massive
{
	[Il2CppSetOption(Option.NullChecks, false)]
	[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
	[Il2CppSetOption(Option.DivideByZeroChecks, false)]
	public static class RegistryViewExtensions
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static View View(this Registry registry)
		{
			return new View(registry, registry.Config.PackingWhenIterating);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static View View(this Registry registry, Packing packingWhenIterating)
		{
			return new View(registry, packingWhenIterating);
		}
	}
}
