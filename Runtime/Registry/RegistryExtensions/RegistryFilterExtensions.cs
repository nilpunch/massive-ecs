using System.Runtime.CompilerServices;

namespace Massive
{
	public static class RegistryFilterExtensions
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static IFilter Filter<TInclude>(this IRegistry registry)
			where TInclude : IIncludeSelector, new()
		{
			return registry.FilterRegistry.Get<TInclude, None>(registry.SetRegistry);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static IFilter Filter<TInclude, TExclude>(this IRegistry registry)
			where TInclude : IIncludeSelector, new()
			where TExclude : IExcludeSelector, new()
		{
			return registry.FilterRegistry.Get<TInclude, TExclude>(registry.SetRegistry);
		}
	}
}
