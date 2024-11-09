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

		public SparseSet[] Included { get; }
		public SparseSet[] Excluded { get; }

		public Filter(SparseSet[] included, SparseSet[] excluded)
		{
			ThrowIfConflicting(included, excluded, "Conflicting include and exclude filter!");

			Included = included;
			Excluded = excluded;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool ContainsId(int id)
		{
			return id >= 0 && (Included.Length == 0 || SetHelpers.AssignedInAll(id, Included)) && (Excluded.Length == 0 || SetHelpers.NotAssignedInAll(id, Excluded));
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
