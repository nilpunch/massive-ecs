using System.Runtime.CompilerServices;

namespace Massive
{
	public static class RegistryGroupExtensions
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static IGroup Group<TOwned>(this IRegistry registry)
			where TOwned : IOwnSelector, new()
		{
			return registry.Group<TOwned, None, None>();
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static IGroup Group<TOwned, TInclude>(this IRegistry registry)
			where TOwned : IOwnSelector, new()
			where TInclude : IIncludeSelector, new()
		{
			return registry.Group<TOwned, TInclude, None>();
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static IGroup Group<TOwned, TInclude, TExclude>(this IRegistry registry)
			where TOwned : IOwnSelector, new()
			where TInclude : IIncludeSelector, new()
			where TExclude : IExcludeSelector, new()
		{
			return registry.GroupRegistry.Get<TOwned, TInclude, TExclude>(registry.SetRegistry);
		}
	}
}
