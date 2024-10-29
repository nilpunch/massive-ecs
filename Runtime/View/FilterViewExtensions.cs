using System.Runtime.CompilerServices;

namespace Massive
{
	public static class FilterViewExtensions
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static FilterView Include<T>(this View view)
		{
			return view.Filter<Include<T>, None>();
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static FilterView Include<T1, T2>(this View view)
		{
			return view.Filter<Include<T1, T2>, None>();
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static FilterView Include<T1, T2, T3>(this View view)
		{
			return view.Filter<Include<T1, T2, T3>, None>();
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static FilterView Include<T1, T2, T3, TInclude>(this View view)
			where TInclude : IIncludeSelector, new()
		{
			return view.Filter<Include<T1, T2, T3, TInclude>, None>();
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static FilterView Exclude<T>(this View view)
		{
			return view.Filter<None, Exclude<T>>();
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static FilterView Exclude<T1, T2>(this View view)
		{
			return view.Filter<None, Exclude<T1, T2>>();
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static FilterView Exclude<T1, T2, T3>(this View view)
		{
			return view.Filter<None, Exclude<T1, T2, T3>>();
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static FilterView Exclude<T1, T2, T3, TExclude>(this View view)
			where TExclude : IExcludeSelector, new()
		{
			return view.Filter<None, Exclude<T1, T2, T3, TExclude>>();
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static FilterView Filter(this View view, Filter filter = null)
		{
			return new FilterView(view.Registry, filter);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static FilterView Filter<TInclude>(this View view)
			where TInclude : IIncludeSelector, new()
		{
			return view.Filter<TInclude, None>();
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static FilterView Filter<TInclude, TExclude>(this View view)
			where TInclude : IIncludeSelector, new()
			where TExclude : IExcludeSelector, new()
		{
			return new FilterView(view.Registry, view.Registry.Filter<TInclude, TExclude>());
		}
	}
}
