using System;
using System.Buffers;

namespace Massive
{
	public static class FilterExtensions
	{
		public static TrimmedFilter Trim(this Filter filter, SparseSet includedSet)
		{
			if (filter.IncludedCount == 0 || filter.IncludedCount == 1 && filter.Included[0] == includedSet)
			{
				return new TrimmedFilter(Array.Empty<SparseSet>(), 0, false, filter.Excluded, filter.ExcludedCount);
			}

			var includedSetIndex = Array.IndexOf(filter.Included, includedSet, 0, filter.IncludedCount);
			if (includedSetIndex == -1)
			{
				return new TrimmedFilter(filter.Included, filter.IncludedCount, false, filter.Excluded, filter.ExcludedCount);
			}
			else
			{
				var includedRented = ArrayPool<SparseSet>.Shared.Rent(filter.IncludedCount);
				Array.Copy(filter.Included, includedRented, filter.IncludedCount);
				includedRented[includedSetIndex] = includedRented[filter.IncludedCount - 1];
				return new TrimmedFilter(includedRented, filter.IncludedCount - 1, true, filter.Excluded, filter.ExcludedCount);
			}
		}

		public static TrimmedFilter AsTrimmed(this Filter filter)
		{
			return new TrimmedFilter(filter.Included, filter.IncludedCount, false, filter.Excluded, filter.ExcludedCount);
		}
	}
}
