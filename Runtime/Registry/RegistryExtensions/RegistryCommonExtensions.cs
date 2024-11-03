using System.Runtime.CompilerServices;

namespace Massive
{
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
