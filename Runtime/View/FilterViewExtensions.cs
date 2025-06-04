using System.Runtime.CompilerServices;
using Unity.IL2CPP.CompilerServices;

namespace Massive
{
	[Il2CppSetOption(Option.NullChecks, false)]
	[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
	public static class FilterViewExtensions
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static FilterView Include<T>(this View view)
		{
			return new FilterView(view.World, view.World.GetFilter<Include<T>, None>(), view.PackingWhenIterating);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static FilterView Include<T1, T2>(this View view)
		{
			return new FilterView(view.World, view.World.GetFilter<Include<T1, T2>, None>(), view.PackingWhenIterating);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static FilterView Include<T1, T2, T3>(this View view)
		{
			return new FilterView(view.World, view.World.GetFilter<Include<T1, T2, T3>, None>(), view.PackingWhenIterating);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static FilterView Include<T1, T2, T3, TInclude>(this View view)
			where TInclude : IIncludeSelector, new()
		{
			return new FilterView(view.World, view.World.GetFilter<Include<T1, T2, T3, TInclude>, None>(), view.PackingWhenIterating);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static FilterView Exclude<T>(this View view)
		{
			return new FilterView(view.World, view.World.GetFilter<None, Exclude<T>>(), view.PackingWhenIterating);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static FilterView Exclude<T1, T2>(this View view)
		{
			return new FilterView(view.World, view.World.GetFilter<None, Exclude<T1, T2>>(), view.PackingWhenIterating);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static FilterView Exclude<T1, T2, T3>(this View view)
		{
			return new FilterView(view.World, view.World.GetFilter<None, Exclude<T1, T2, T3>>(), view.PackingWhenIterating);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static FilterView Exclude<T1, T2, T3, TExclude>(this View view)
			where TExclude : IExcludeSelector, new()
		{
			return new FilterView(view.World, view.World.GetFilter<None, Exclude<T1, T2, T3, TExclude>>(), view.PackingWhenIterating);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static FilterView Filter(this View view, Filter filter = null)
		{
			return new FilterView(view.World, filter, view.PackingWhenIterating);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static FilterView Filter<TInclude>(this View view)
			where TInclude : IIncludeSelector, new()
		{
			return new FilterView(view.World, view.World.GetFilter<TInclude, None>(), view.PackingWhenIterating);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static FilterView Filter<TInclude, TExclude>(this View view)
			where TInclude : IIncludeSelector, new()
			where TExclude : IExcludeSelector, new()
		{
			return new FilterView(view.World, view.World.GetFilter<TInclude, TExclude>(), view.PackingWhenIterating);
		}
	}
}
