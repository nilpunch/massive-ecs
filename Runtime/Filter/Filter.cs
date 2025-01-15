using System;
using System.Runtime.CompilerServices;
using Unity.IL2CPP.CompilerServices;

namespace Massive
{
	[Il2CppSetOption(Option.NullChecks, false)]
	[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
	[Il2CppSetOption(Option.DivideByZeroChecks, false)]
	public class Filter
	{
		public static Filter Empty { get; } = new Filter(Array.Empty<SparseSet>(), Array.Empty<SparseSet>());

		public int IncludedCount { get; protected set; }
		public int ExcludedCount { get; protected set; }

		public SparseSet[] Included { get; protected set; }
		public SparseSet[] Excluded { get; protected set; }

		public ReducedFilter NotReduced { get; private set; }
		private ReducedFilter[] ReducedFilters { get; set; } = Array.Empty<ReducedFilter>();

		public Filter(SparseSet[] included, SparseSet[] excluded)
		{
			ThrowIfConflicting(included, excluded, "Conflicting include and exclude filter!");

			Included = included;
			Excluded = excluded;
			IncludedCount = included.Length;
			ExcludedCount = excluded.Length;

			UpdateReducedFilters();
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool ContainsId(int id)
		{
			return (SetUtils.NonNegativeIfAssignedInAll(id, Included, IncludedCount)
				| ~SetUtils.NegativeIfNotAssignedInAll(id, Excluded, ExcludedCount)) >= 0;
		}

		public ReducedFilter ReduceIncluded(SparseSet included)
		{
			for (var i = 0; i < ReducedFilters.Length; i++)
			{
				var reducedFilter = ReducedFilters[i];
				if (reducedFilter.Reduced == included)
				{
					return reducedFilter;
				}
			}

			return NotReduced;
		}

		protected void UpdateReducedFilters()
		{
			NotReduced = ReducedFilter.Create(this);

			if (IncludedCount > ReducedFilters.Length)
			{
				ReducedFilters = ReducedFilters.Resize(IncludedCount);
				for (var i = 0; i < IncludedCount; i++)
				{
					ReducedFilters[i] = ReducedFilter.Create(this, Included[i]);
				}
			}
		}

		public static void ThrowIfConflicting(SparseSet[] included, SparseSet[] excluded, string errorMessage)
		{
			if (included.ContainsAny(excluded))
			{
				throw new Exception(errorMessage);
			}
		}
	}
}
