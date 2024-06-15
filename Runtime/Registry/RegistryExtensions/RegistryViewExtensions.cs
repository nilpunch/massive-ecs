using System.Runtime.CompilerServices;

namespace Massive
{
	public static class RegistryViewExtensions
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static View View(this IRegistry registry)
		{
			return new View(registry);
		}
	}
}
