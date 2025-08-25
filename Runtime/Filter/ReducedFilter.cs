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
		public Masks Masks { get; }

		public long[] IncludeMask { get; }
		public int IncludeStart { get; }
		public int IncludeEnd { get; }

		public long[] ExcludeMask { get; }
		public int ExcludeStart { get; }
		public int ExcludeEnd { get; }

		public ReducedFilter(SparseSet[] included, int includedLength, SparseSet[] excluded, int excludedLength, SparseSet reduced, Masks masks)
		{
			IncludeStart = includedLength == 0 ? 0 : int.MaxValue;
			IncludeEnd = 0;
			ExcludeStart = excludedLength == 0 ? 0 : int.MaxValue;
			ExcludeEnd = 0;
			Reduced = reduced;
			Masks = masks;

			for (var i = 0; i < includedLength; i++)
			{
				var componentId = included[i].ComponentId;
				IncludeStart = MathUtils.Min(componentId >> 6, IncludeStart);
				IncludeEnd = MathUtils.Max((componentId >> 6) + 1, IncludeEnd);
			}

			for (var i = 0; i < excludedLength; i++)
			{
				var componentId = excluded[i].ComponentId;
				ExcludeStart = MathUtils.Min(componentId >> 6, ExcludeStart);
				ExcludeEnd = MathUtils.Max((componentId >> 6) + 1, ExcludeEnd);
			}

			IncludeMask = new long[IncludeEnd];
			for (var i = 0; i < includedLength; i++)
			{
				var componentId = included[i].ComponentId;
				IncludeMask[componentId >> 6] |= 1L << componentId;
			}

			ExcludeMask = new long[ExcludeEnd];
			for (var i = 0; i < excludedLength; i++)
			{
				var componentId = excluded[i].ComponentId;
				ExcludeMask[componentId >> 6] |= 1L << componentId;
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool ContainsId(int id)
		{
			var shouldNotBecomeZero = (long)id;
			var shouldStayNegative = (long)~id;
			var bitmap = Masks.BitMap;
			var maskOffset = id * Masks.MaskLength;

			for (var i = IncludeStart; i < IncludeEnd; i++)
			{
				shouldNotBecomeZero |= (IncludeMask[i] & bitmap[maskOffset + i]) - 1;
			}

			for (var i = ExcludeStart; i < ExcludeEnd; i++)
			{
				shouldStayNegative &= (ExcludeMask[i] & bitmap[maskOffset + i]) - 1;
			}

			return (shouldNotBecomeZero | ~shouldStayNegative) >= 0L;
		}

		public static ReducedFilter Create(Filter filter, SparseSet reduced = null)
		{
			if (filter.IncludedCount == 0 || filter.IncludedCount == 1 && filter.Included[0] == reduced)
			{
				return new ReducedFilter(Array.Empty<SparseSet>(), 0, filter.Excluded, filter.ExcludedCount, null, filter.Masks);
			}

			var reducedIndex = Array.IndexOf(filter.Included, reduced, 0, filter.IncludedCount);
			if (reducedIndex == -1)
			{
				return new ReducedFilter(filter.Included, filter.IncludedCount, filter.Excluded, filter.ExcludedCount, null, filter.Masks);
			}
			else if (reducedIndex == filter.IncludedCount - 1)
			{
				return new ReducedFilter(filter.Included, filter.IncludedCount - 1, filter.Excluded, filter.ExcludedCount, filter.Included[^1], filter.Masks);
			}
			else
			{
				var included = new SparseSet[filter.IncludedCount - 1];
				Array.Copy(filter.Included, included, filter.IncludedCount - 1);
				included[reducedIndex] = filter.Included[^1];
				return new ReducedFilter(included, filter.IncludedCount - 1, filter.Excluded, filter.ExcludedCount, filter.Included[reducedIndex], filter.Masks);
			}
		}
	}
}
