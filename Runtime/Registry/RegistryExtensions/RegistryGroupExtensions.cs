using System.Runtime.CompilerServices;

namespace Massive
{
	public static class RegistryGroupExtensions
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Group Group<TInclude>(this Registry registry)
			where TInclude : IIncludeSelector, new()
		{
			return registry.GroupRegistry.Get<TInclude, None, None>();
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Group Group<TInclude, TExclude>(this Registry registry)
			where TInclude : IIncludeSelector, new()
			where TExclude : IExcludeSelector, new()
		{
			return registry.GroupRegistry.Get<TInclude, TExclude, None>();
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Group Group<TInclude, TExclude, TOwned>(this Registry registry)
			where TInclude : IIncludeSelector, new()
			where TExclude : IExcludeSelector, new()
			where TOwned : IOwnSelector, new()
		{
			return registry.GroupRegistry.Get<TInclude, TExclude, TOwned>();
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Group Group(this Registry registry, SparseSet[] included = null, SparseSet[] excluded = null, SparseSet[] owned = null)
		{
			return registry.GroupRegistry.Get(included, excluded, owned);
		}
	}
}
