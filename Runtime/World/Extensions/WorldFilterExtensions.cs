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
		public static Filter GetFilter<TInclude>(this World world)
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
		public static Filter GetFilter<TInclude, TExclude>(this World world)
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

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static FilterView Include<T>(this World world)
		{
			return new FilterView(world, world.GetFilter<Include<T>, None>(), world.Config.PackingWhenIterating);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static FilterView Include<T1, T2>(this World world)
		{
			return new FilterView(world, world.GetFilter<Include<T1, T2>, None>(), world.Config.PackingWhenIterating);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static FilterView Include<T1, T2, T3>(this World world)
		{
			return new FilterView(world, world.GetFilter<Include<T1, T2, T3>, None>(), world.Config.PackingWhenIterating);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static FilterView Include<T1, T2, T3, TInclude>(this World world)
			where TInclude : IIncludeSelector, new()
		{
			return new FilterView(world, world.GetFilter<Include<T1, T2, T3, TInclude>, None>(), world.Config.PackingWhenIterating);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static FilterView Exclude<T>(this World world)
		{
			return new FilterView(world, world.GetFilter<None, Exclude<T>>(), world.Config.PackingWhenIterating);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static FilterView Exclude<T1, T2>(this World world)
		{
			return new FilterView(world, world.GetFilter<None, Exclude<T1, T2>>(), world.Config.PackingWhenIterating);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static FilterView Exclude<T1, T2, T3>(this World world)
		{
			return new FilterView(world, world.GetFilter<None, Exclude<T1, T2, T3>>(), world.Config.PackingWhenIterating);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static FilterView Exclude<T1, T2, T3, TExclude>(this World world)
			where TExclude : IExcludeSelector, new()
		{
			return new FilterView(world, world.GetFilter<None, Exclude<T1, T2, T3, TExclude>>(), world.Config.PackingWhenIterating);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static FilterView Filter(this World world, Filter filter = null)
		{
			return new FilterView(world, filter, world.Config.PackingWhenIterating);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static FilterView Filter<TInclude>(this World world)
			where TInclude : IIncludeSelector, new()
		{
			return new FilterView(world, world.GetFilter<TInclude, None>(), world.Config.PackingWhenIterating);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static FilterView Filter<TInclude, TExclude>(this World world)
			where TInclude : IIncludeSelector, new()
			where TExclude : IExcludeSelector, new()
		{
			return new FilterView(world, world.GetFilter<TInclude, TExclude>(), world.Config.PackingWhenIterating);
		}
	}
}
