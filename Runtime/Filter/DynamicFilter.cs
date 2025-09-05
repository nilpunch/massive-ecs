#if !MASSIVE_DISABLE_ASSERT
#define MASSIVE_ASSERT
#endif

using System;
using Unity.IL2CPP.CompilerServices;

namespace Massive
{
	[Il2CppSetOption(Option.NullChecks, false)]
	[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
	public class DynamicFilter : Filter
	{
		public BitSets BitSets { get; }

		public DynamicFilter(World world) : base(Array.Empty<BitSet>(), Array.Empty<BitSet>())
		{
			BitSets = world.BitSets;
		}

		public DynamicFilter Include<T>()
		{
			var set = BitSets.Get<T>();

			ConflictingFilterException.ThrowIfConflictWithExcluded(this, set);

			if (Included.Contains(set))
			{
				return this;
			}

			if (IncludedCount >= Included.Length)
			{
				Included = Included.ResizeToNextPowOf2(IncludedCount + 1);
			}

			Included[IncludedCount] = set;
			IncludedCount += 1;

			return this;
		}

		public DynamicFilter Exclude<T>()
		{
			var set = BitSets.Get<T>();

			ConflictingFilterException.ThrowIfConflictWithIncluded(this, set);

			if (Excluded.Contains(set))
			{
				return this;
			}

			if (ExcludedCount >= Excluded.Length)
			{
				Excluded = Excluded.ResizeToNextPowOf2(ExcludedCount + 1);
			}

			Excluded[ExcludedCount] = set;
			ExcludedCount += 1;

			return this;
		}
	}
}
