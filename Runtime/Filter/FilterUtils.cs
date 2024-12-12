using System;

namespace Massive
{
	public static class FilterUtils
	{
		public static ReducedFilter MakeReducedFilter(Filter filter, SparseSet ignore)
		{
			if (filter.IncludedCount == 0 || filter.IncludedCount == 1 && filter.Included[0] == ignore)
			{
				return new ReducedFilter(Array.Empty<SparseSet>(), 0, filter.Excluded, filter.ExcludedCount, null);
			}

			var ignoredIndex = Array.IndexOf(filter.Included, ignore, 0, filter.IncludedCount);
			if (ignoredIndex == -1)
			{
				return new ReducedFilter(filter.Included, filter.IncludedCount, filter.Excluded, filter.ExcludedCount, null);
			}
			else if (ignoredIndex == filter.IncludedCount - 1)
			{
				return new ReducedFilter(filter.Included, filter.IncludedCount - 1, filter.Excluded, filter.ExcludedCount, filter.Included[^1]);
			}
			else
			{
				var included = new SparseSet[filter.IncludedCount - 1];
				Array.Copy(filter.Included, included, filter.IncludedCount - 1);
				included[ignoredIndex] = filter.Included[^1];
				return new ReducedFilter(included, filter.IncludedCount - 1, filter.Excluded, filter.ExcludedCount, filter.Included[ignoredIndex]);
			}
		}

		public static ReducedFilter MakeReducedFilter(Filter filter)
		{
			return new ReducedFilter(filter.Included, filter.IncludedCount, filter.Excluded, filter.ExcludedCount, null);
		}
	}
}
