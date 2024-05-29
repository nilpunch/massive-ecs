using System;
using System.Runtime.CompilerServices;

namespace Massive
{
	public interface IGroupSelector
	{
		public int OwnedCount { get; }
		public int IncludeCount { get; }
		public int ExcludeCount { get; }

		void Select(ArraySegment<ISet> owned, ArraySegment<IReadOnlySet> include, ArraySegment<IReadOnlySet> exclude);
	}

	public readonly struct GroupSelector<TOwned, TInclude, TExclude> : IGroupSelector
		where TOwned : struct, ISetSelector
		where TInclude : struct, IReadOnlySetSelector
		where TExclude : struct, IReadOnlySetSelector
	{
		private readonly SetRegistry _setRegistry;

		public GroupSelector(SetRegistry setRegistry)
		{
			_setRegistry = setRegistry;
		}

		public int OwnedCount
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => default(TOwned).Count;
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
		public void Select(ArraySegment<ISet> owned, ArraySegment<IReadOnlySet> include, ArraySegment<IReadOnlySet> exclude)
		{
			default(TOwned).Select(_setRegistry, owned);
			default(TInclude).Select(_setRegistry, include);
			default(TExclude).Select(_setRegistry, exclude);
		}
	}
}
