using System.Runtime.CompilerServices;

namespace Massive
{
	public static class RegistryFilterExtensions
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static IFilter Filter<TInclude>(this IRegistry registry)
			where TInclude : struct, IIncludeSelector
		{
			return registry.FilterRegistry.Get<TInclude, None>(registry.SetRegistry);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static IFilter Filter<TInclude, TExclude>(this IRegistry registry)
			where TInclude : struct, IIncludeSelector
			where TExclude : struct, IExcludeSelector
		{
			return registry.FilterRegistry.Get<TInclude, TExclude>(registry.SetRegistry);
		}
	}
}
