using System;
using System.Buffers;

namespace Massive
{
	public static class FilterExtensions
	{
		public static TrimmedFilter Trim(this Filter filter, SparseSet includedSet)
		{
			if (filter.Included.Length == 0 || filter.Included.Length == 1 && filter.Included[0] == includedSet)
			{
				return new TrimmedFilter(Array.Empty<SparseSet>(), 0, false, filter.Excluded);
			}

			var includedSetIndex = Array.IndexOf(filter.Included, includedSet);
			if (includedSetIndex == -1)
			{
				return new TrimmedFilter(filter.Included, filter.Included.Length, false, filter.Excluded);
			}
			else
			{
				var includedRented = ArrayPool<SparseSet>.Shared.Rent(filter.Included.Length);
				Array.Copy(filter.Included, includedRented, filter.Included.Length);
				includedRented[includedSetIndex] = includedRented[filter.Included.Length - 1];
				return new TrimmedFilter(includedRented, filter.Included.Length - 1, true, filter.Excluded);
			}
		}

		public static TrimmedFilter ToTrimmed(this Filter filter)
		{
			return new TrimmedFilter(filter.Included, filter.Included.Length, false, filter.Excluded);
		}
	}
}
