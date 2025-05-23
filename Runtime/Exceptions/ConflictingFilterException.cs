using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Massive
{
	public class ConflictingFilterException : MassiveException
	{
		public enum FilterType
		{
			Include,
			Exclude
		}

		private ConflictingFilterException(string message) : base(message)
		{
		}

		[Conditional(Condition)]
		[MethodImpl(MethodImplOptions.NoInlining)]
		public static void ThrowIfHasConflicts(SparseSet[] included, SparseSet[] excluded)
		{
			if (included.ContainsAny(excluded))
			{
				throw new ConflictingFilterException("Filter has conflicting included and excluded components.");
			}
		}

		[Conditional(Condition)]
		[MethodImpl(MethodImplOptions.NoInlining)]
		public static void ThrowIfHasDuplicates(SparseSet[] sets, FilterType filterType)
		{
			for (var i = 0; i < sets.Length; i++)
			{
				var set = sets[i];
				for (var j = i + 1; j < sets.Length; j++)
				{
					if (set == sets[j])
					{
						var filterName = filterType switch
						{
							FilterType.Include => "Included",
							FilterType.Exclude => "Excluded",
							_ => throw new ArgumentOutOfRangeException(nameof(filterType), filterType, null)
						};

						throw new ConflictingFilterException($"{filterName} contains duplicate sets.");
					}
				}
			}
		}

		[Conditional(Condition)]
		[MethodImpl(MethodImplOptions.NoInlining)]
		public static void ThrowIfCantInclude<T>(Filter filter, SparseSet sparseSet)
		{
			if (filter.Excluded.Contains(sparseSet))
			{
				throw new ConflictingFilterException($"You are trying include a set of type:{typeof(T).GetGenericName()} while filter want to exclude it.");
			}
		}

		[Conditional(Condition)]
		[MethodImpl(MethodImplOptions.NoInlining)]
		public static void ThrowIfConflictWithIncluded(Filter filter, SparseSet sparseSet)
		{
			if (filter.Excluded.Contains(sparseSet))
			{
				throw new ConflictingFilterException("Conflict with excluded sets.");
			}
		}

		[Conditional(Condition)]
		[MethodImpl(MethodImplOptions.NoInlining)]
		public static void ThrowIfConflictWithExcluded(Filter filter, SparseSet sparseSet)
		{
			if (filter.Included.Contains(sparseSet))
			{
				throw new ConflictingFilterException("Conflict with included sets.");
			}
		}
	}
}
