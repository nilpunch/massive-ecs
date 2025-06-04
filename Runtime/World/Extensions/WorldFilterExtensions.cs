using System;
using System.Runtime.CompilerServices;
using Unity.IL2CPP.CompilerServices;

namespace Massive
{
	[Il2CppSetOption(Option.NullChecks, false)]
	[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
	public static class WorldFilterExtensions
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Filter Filter<TInclude>(this World world)
			where TInclude : IIncludeSelector, new()
		{
			var info = TypeId<Tuple<TInclude, None>>.Info;

			var filters = world.Filters;

			filters.EnsureLookupAt(info.Index);
			var candidate = filters.Lookup[info.Index];

			if (candidate != null)
			{
				return candidate;
			}

			var included = new TInclude().Select(world.Sets);
			var excluded = Array.Empty<SparseSet>();

			var filter = filters.Get(included, excluded);

			filters.Lookup[info.Index] = filter;

			return filter;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Filter Filter<TInclude, TExclude>(this World world)
			where TInclude : IIncludeSelector, new()
			where TExclude : IExcludeSelector, new()
		{
			var info = TypeId<Tuple<TInclude, TExclude>>.Info;

			var filters = world.Filters;

			filters.EnsureLookupAt(info.Index);
			var candidate = filters.Lookup[info.Index];

			if (candidate != null)
			{
				return candidate;
			}

			var included = new TInclude().Select(world.Sets);
			var excluded = new TExclude().Select(world.Sets);

			var filter = filters.Get(included, excluded);

			filters.Lookup[info.Index] = filter;

			return filter;
		}
	}
}
