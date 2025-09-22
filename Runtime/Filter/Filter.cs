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

		public int IncludeCount { get; internal set; }
		public int ExcludeCount { get; internal set; }
		public int AnyCount { get; internal set; }

		public BitSet[] Included { get; internal set; }
		public BitSet[] Excluded { get; internal set; }
		public BitSet[] Any { get; internal set; }

		public Filter(BitSet[] included, BitSet[] excluded, BitSet[] any)
		{
			FilterException.ThrowIfHasConflicts(included, excluded, FilterType.Included, FilterType.Excluded);
			FilterException.ThrowIfHasConflicts(included, any, FilterType.Included, FilterType.Any);
			FilterException.ThrowIfHasConflicts(excluded, any, FilterType.Excluded, FilterType.Any);

			Included = included;
			Excluded = excluded;
			Any = any;
			IncludeCount = included.Length;
			ExcludeCount = excluded.Length;
			AnyCount = any.Length;
		}

		public void SetIncluded(BitSet[] all)
		{
			Included = all;
			IncludeCount = all.Length;

			FilterException.ThrowIfHasConflicts(Included, Excluded, FilterType.Included, FilterType.Excluded);
			FilterException.ThrowIfHasConflicts(Included, Any, FilterType.Included, FilterType.Any);
		}

		public void SetExcluded(BitSet[] none)
		{
			Excluded = none;
			ExcludeCount = none.Length;

			FilterException.ThrowIfHasConflicts(Included, Excluded, FilterType.Included, FilterType.Excluded);
			FilterException.ThrowIfHasConflicts(Excluded, Any, FilterType.Excluded, FilterType.Any);
		}

		public void SetAny(BitSet[] any)
		{
			Any = any;
			AnyCount = any.Length;

			FilterException.ThrowIfHasConflicts(Included, Any, FilterType.Included, FilterType.Any);
			FilterException.ThrowIfHasConflicts(Excluded, Any, FilterType.Excluded, FilterType.Any);
		}
	}
}
