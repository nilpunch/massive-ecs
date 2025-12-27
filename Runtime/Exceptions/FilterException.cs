using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Massive
{
	public enum FilterType
	{
		Included,
		Excluded,
		Any,
	}

	public class FilterException : MassiveException
	{
		private FilterException(string message) : base(message)
		{
		}

		[Conditional(Condition)]
		[MethodImpl(MethodImplOptions.NoInlining)]
		public static void ThrowIfCantSetFilter(BitSet[] newFilter, BitSet[] existingFilter)
		{
			if (newFilter == null || newFilter.Length == 0)
			{
				throw new FilterException($"Trying to set empty or null filter.");
			}

			if (existingFilter.Length != 0)
			{
				throw new FilterException($"Trying to override existing filter.");
			}
		}

		[Conditional(Condition)]
		[MethodImpl(MethodImplOptions.NoInlining)]
		public static void ThrowIfHasConflicts(BitSet[] setsA, BitSet[] setsB, FilterType typeA, FilterType typeB)
		{
			for (var i = 0; i < setsA.Length; i++)
			{
				for (var j = 0; j < setsB.Length; j++)
				{
					if (setsA[i] != null && setsB[j] != null && setsA[i] == setsB[j])
					{
						throw new FilterException($"Query has conflicting {Name(typeA)} and {Name(typeB)} components.");

						string Name(FilterType type) =>
							type switch
							{
								FilterType.Included => "Included",
								FilterType.Excluded => "Excluded",
								FilterType.Any => "Any",
								_ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
							};
					}
				}
			}
		}

		[Conditional(Condition)]
		[MethodImpl(MethodImplOptions.NoInlining)]
		public static void ThrowIfCantQuery<T>(Filter filter, BitSet bitSet)
		{
			if (filter.Excluded.Contains(bitSet))
			{
				throw new FilterException($"You are trying query a component:{typeof(T).GetGenericName()} while filter marks it as None.");
			}
		}
	}
}
