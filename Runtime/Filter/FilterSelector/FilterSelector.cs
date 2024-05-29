using System;
using System.Runtime.CompilerServices;

namespace Massive
{
	public readonly struct FilterSelector<TInclude, TExclude> : IFilterSelector
		where TInclude : struct, IReadOnlySetSelector
		where TExclude : struct, IReadOnlySetSelector
	{
		private readonly SetRegistry _setRegistry;

		public FilterSelector(SetRegistry setRegistry)
		{
			_setRegistry = setRegistry;
		}

		public int IncludeCount
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => default(TInclude).Count;
		}

		public int ExcludeCount
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => default(TExclude).Count;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Select(ArraySegment<IReadOnlySet> include, ArraySegment<IReadOnlySet> exclude)
		{
			default(TInclude).Select(_setRegistry, include);
			default(TExclude).Select(_setRegistry, exclude);
		}
	}
}
