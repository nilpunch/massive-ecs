using System;
using System.Runtime.CompilerServices;
using Unity.IL2CPP.CompilerServices;

namespace Massive
{
	[Il2CppSetOption(Option.NullChecks, false)]
	[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
	public readonly struct ReducedFilter
	{
		public SparseSet Reduced { get; }

		public SparseSet[] Included { get; }
		public int IncludedLength { get; }

		public SparseSet[] Excluded { get; }
		public int ExcludedLength { get; }

		public ReducedFilter(SparseSet[] included, int includedLength, SparseSet[] excluded, int excludedLength, SparseSet reduced)
		{
			Included = included;
			IncludedLength = includedLength;
			Excluded = excluded;
			ExcludedLength = excludedLength;
			Reduced = reduced;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool ContainsId(int id)
		{
			return (SetUtils.NonNegativeIfAssignedInAll(id, Included, IncludedLength)
				| ~SetUtils.NegativeIfNotAssignedInAll(id, Excluded, ExcludedLength)) >= 0;
		}

		public static ReducedFilter Create(Filter filter, SparseSet reduced = null)
		{
			if (filter.IncludedCount == 0 || filter.IncludedCount == 1 && filter.Included[0] == reduced)
			{
				return new ReducedFilter(Array.Empty<SparseSet>(), 0, filter.Excluded, filter.ExcludedCount, null);
			}

			var reducedIndex = Array.IndexOf(filter.Included, reduced, 0, filter.IncludedCount);
			if (reducedIndex == -1)
			{
				return new ReducedFilter(filter.Included, filter.IncludedCount, filter.Excluded, filter.ExcludedCount, null);
			}
			else if (reducedIndex == filter.IncludedCount - 1)
			{
				return new ReducedFilter(filter.Included, filter.IncludedCount - 1, filter.Excluded, filter.ExcludedCount, filter.Included[^1]);
			}
			else
			{
				var included = new SparseSet[filter.IncludedCount - 1];
				Array.Copy(filter.Included, included, filter.IncludedCount - 1);
				included[reducedIndex] = filter.Included[^1];
				return new ReducedFilter(included, filter.IncludedCount - 1, filter.Excluded, filter.ExcludedCount, filter.Included[reducedIndex]);
			}
		}
	}
}
