using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Massive
{
	public class ConflictingFilterException : MassiveException
	{
		public enum FilterType
		{
			Include,
			Exclude,
			Both,
		}

		private ConflictingFilterException(string message) : base(message)
		{
		}

		[Conditional(Condition)]
		[MethodImpl(MethodImplOptions.NoInlining)]
		public static void ThrowIfHasConflicts(BitSet[] included, BitSet[] excluded)
		{
			for (var i = 0; i < included.Length; i++)
			{
				for (var j = 0; j < excluded.Length; j++)
				{
					if (included[i] == excluded[i])
					{
						throw new ConflictingFilterException("Filter has conflicting included and excluded components.");
					}
				}
			}
		}

		[Conditional(Condition)]
		[MethodImpl(MethodImplOptions.NoInlining)]
		public static void ThrowIfHasDuplicates(BitSet[] sets)
		{
			for (var i = 0; i < sets.Length; i++)
			{
				var set = sets[i];
				for (var j = i + 1; j < sets.Length; j++)
				{
					if (set == sets[j])
					{
						throw new ConflictingFilterException($"Component selection contains duplicate sets.");
					}
				}
			}
		}

		[Conditional(Condition)]
		[MethodImpl(MethodImplOptions.NoInlining)]
		public static void ThrowIfCantInclude<T>(Filter filter, BitSet bitSet)
		{
			if (filter.None.Contains(bitSet))
			{
				throw new ConflictingFilterException($"You are trying include a set of type:{typeof(T).GetGenericName()} while filter want to exclude it.");
			}
		}

		[Conditional(Condition)]
		[MethodImpl(MethodImplOptions.NoInlining)]
		public static void ThrowIfConflictWithIncluded(Filter filter, BitSet bitSet)
		{
			if (filter.None.Contains(bitSet))
			{
				throw new ConflictingFilterException("Conflict with excluded sets.");
			}
		}

		[Conditional(Condition)]
		[MethodImpl(MethodImplOptions.NoInlining)]
		public static void ThrowIfConflictWithExcluded(Filter filter, BitSet bitSet)
		{
			if (filter.All.Contains(bitSet))
			{
				throw new ConflictingFilterException("Conflict with included sets.");
			}
		}
	}
}
