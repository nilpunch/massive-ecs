using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Massive
{
	public enum FilterType
	{
		All,
		None,
		Any,
	}

	public class FilterException : MassiveException
	{
		private FilterException(string message) : base(message)
		{
		}

		[Conditional(Condition)]
		[MethodImpl(MethodImplOptions.NoInlining)]
		public static void ThrowIfHasConflicts(BitSet[] setsA, BitSet[] setsB, FilterType typeA, FilterType typeB)
		{
			for (var i = 0; i < setsA.Length; i++)
			{
				for (var j = 0; j < setsB.Length; j++)
				{
					if (setsA[i] != null && setsB[i] != null && setsA[i] == setsB[i])
					{
						throw new FilterException($"Query has conflicting {Name(typeA)} and {Name(typeB)} components.");

						string Name(FilterType type) =>
							type switch
							{
								FilterType.All => "All",
								FilterType.None => "None",
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
			if (filter.None.Contains(bitSet))
			{
				throw new FilterException($"You are trying query a component:{typeof(T).GetGenericName()} while filter marks it as None.");
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
						throw new FilterException($"Component selection contains duplicate sets.");
					}
				}
			}
		}
	}
}
