using System.Runtime.CompilerServices;

namespace Massive
{
	public static class RegistryFilterExtensions
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Filter Filter<TInclude>(this Registry registry)
			where TInclude : IIncludeSelector, new()
		{
			return registry.Filter<TInclude, None>();
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Filter Filter<TInclude, TExclude>(this Registry registry)
			where TInclude : IIncludeSelector, new()
			where TExclude : IExcludeSelector, new()
		{
			return registry.FilterRegistry.Get<TInclude, TExclude>();
		}
	}
}
