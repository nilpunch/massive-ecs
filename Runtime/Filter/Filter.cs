#if !MASSIVE_DISABLE_ASSERT
#define MASSIVE_ASSERT
#endif

using System;
using System.Runtime.CompilerServices;
using Unity.IL2CPP.CompilerServices;

namespace Massive
{
	[Il2CppSetOption(Option.NullChecks, false)]
	[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
	public class Filter
	{
		public static Filter Empty { get; } = new Filter();

		public int IncludedCount { get; protected set; }
		public int ExcludedCount { get; protected set; }

		public BitSet[] Included { get; protected set; }
		public BitSet[] Excluded { get; protected set; }

		public Filter()
			: this(Array.Empty<BitSet>(), Array.Empty<BitSet>())
		{
		}

		public Filter(BitSet[] included, BitSet[] excluded)
		{
			ConflictingFilterException.ThrowIfHasConflicts(included, excluded);

			Included = included;
			Excluded = excluded;
			IncludedCount = included.Length;
			ExcludedCount = excluded.Length;
		}
	}
}
