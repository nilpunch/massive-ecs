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

		public Filter(BitSet[] included, BitSet[] excluded, BitSet[] any)
		{
			FilterException.ThrowIfHasConflicts(included, excluded, FilterType.Included, FilterType.Excluded);
			FilterException.ThrowIfHasConflicts(included, any, FilterType.Included, FilterType.Any);
			FilterException.ThrowIfHasConflicts(excluded, any, FilterType.Excluded, FilterType.Any);

			Included = included;
			Excluded = excluded;
			IncludeCount = included.Length;
			ExcludeCount = excluded.Length;
			AnyCount = any.Length;
		}

		public void SetIncluded(BitSet[] included)
		{
			FilterException.ThrowIfCantSetFilter(included, Included);

			Included = included;
			IncludeCount = included.Length;

			FilterException.ThrowIfHasConflicts(Included, Excluded, FilterType.Included, FilterType.Excluded);
		}

		public void SetExcluded(BitSet[] excluded)
		{
			FilterException.ThrowIfCantSetFilter(excluded, Excluded);

			Excluded = excluded;
			ExcludeCount = excluded.Length;

			FilterException.ThrowIfHasConflicts(Included, Excluded, FilterType.Included, FilterType.Excluded);
		}
	}
}
