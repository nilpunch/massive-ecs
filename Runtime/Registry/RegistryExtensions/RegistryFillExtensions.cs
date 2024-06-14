using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Massive
{
	public static class RegistryFillExtensions
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void Fill<TInclude>(this IRegistry registry, IList<Entity> result)
			where TInclude : IIncludeSelector, new()
		{
			registry.Fill<TInclude, None>(result);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void Fill<TInclude, TExclude>(this IRegistry registry, IList<Entity> result)
			where TInclude : IIncludeSelector, new()
			where TExclude : IExcludeSelector, new()
		{
			registry.FilterView(registry.Filter<TInclude, TExclude>()).ForEachUniversal(new EntityFillEntitiesInvoker() { Result = result, Entities = registry.Entities });
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void Fill<TInclude>(this IRegistry registry, IList<int> result)
			where TInclude : IIncludeSelector, new()
		{
			registry.Fill<TInclude, None>(result);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void Fill<TInclude, TExclude>(this IRegistry registry, IList<int> result)
			where TInclude : IIncludeSelector, new()
			where TExclude : IExcludeSelector, new()
		{
			registry.FilterView(registry.Filter<TInclude, TExclude>()).ForEachUniversal(new EntityFillInvoker() { Result = result });
		}
	}
}
