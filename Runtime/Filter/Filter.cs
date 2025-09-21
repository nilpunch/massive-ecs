#if !MASSIVE_DISABLE_ASSERT
#define MASSIVE_ASSERT
#endif

using System;
using Unity.IL2CPP.CompilerServices;

namespace Massive
{
	[Il2CppSetOption(Option.NullChecks, false)]
	[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
	public struct Filter
	{
		public static Filter Empty { get; } = new Filter(Array.Empty<BitSet>(), Array.Empty<BitSet>(), Array.Empty<BitSet>());

		public int AllCount { get; internal set; }
		public int NoneCount { get; internal set; }
		public int AnyCount { get; internal set; }

		public BitSet[] All { get; internal set; }
		public BitSet[] None { get; internal set; }
		public BitSet[] Any { get; internal set; }

		public Filter(BitSet[] all, BitSet[] none, BitSet[] any)
		{
			FilterException.ThrowIfHasConflicts(all, none, FilterType.All, FilterType.None);
			FilterException.ThrowIfHasConflicts(all, any, FilterType.All, FilterType.Any);
			FilterException.ThrowIfHasConflicts(none, any, FilterType.None, FilterType.Any);

			All = all;
			None = none;
			Any = any;
			AllCount = all.Length;
			NoneCount = none.Length;
			AnyCount = any.Length;
		}

		public void SetAll(BitSet[] all)
		{
			All = all;
			AllCount = all.Length;

			FilterException.ThrowIfHasConflicts(All, None, FilterType.All, FilterType.None);
			FilterException.ThrowIfHasConflicts(All, Any, FilterType.All, FilterType.Any);
		}

		public void SetNone(BitSet[] none)
		{
			None = none;
			NoneCount = none.Length;

			FilterException.ThrowIfHasConflicts(All, None, FilterType.All, FilterType.None);
			FilterException.ThrowIfHasConflicts(None, Any, FilterType.None, FilterType.Any);
		}

		public void SetAny(BitSet[] any)
		{
			Any = any;
			AnyCount = any.Length;

			FilterException.ThrowIfHasConflicts(All, Any, FilterType.All, FilterType.Any);
			FilterException.ThrowIfHasConflicts(None, Any, FilterType.None, FilterType.Any);
		}
	}
}
