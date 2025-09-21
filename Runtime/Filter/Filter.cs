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
			ConflictingFilterException.ThrowIfHasConflicts(all, none);

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
		}

		public void SetNone(BitSet[] none)
		{
			None = none;
			NoneCount = none.Length;
		}
		
		public void SetAny(BitSet[] any)
		{
			Any = any;
			AnyCount = any.Length;
		}
	}
}
