using System.Runtime.CompilerServices;
using Unity.IL2CPP.CompilerServices;

namespace Massive
{
	[Il2CppSetOption(Option.NullChecks, false)]
	[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
	public static class SetUtils
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int CountAssignedInAll(int id, SparseSet[] sets)
		{
			var counter = sets.Length;
			for (var i = 0; i < sets.Length; i++)
			{
				var index = sets[i].GetIndexOrNegative(id);
				counter -= (int)((uint)index >> 31);
			}
			return counter;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool AssignedInAll(int id, SparseSet[] sets)
		{
			return NonNegativeIfAssignedInAll(id, sets, sets.Length) >= 0;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool NotAssignedInAll(int id, SparseSet[] sets)
		{
			return NegativeIfNotAssignedInAll(id, sets, sets.Length) < 0;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int NonNegativeIfAssignedInAll(int id, SparseSet[] sets, int setsLength)
		{
			var shouldNotBecomeNegative = id;
			for (var i = 0; i < setsLength; i++)
			{
				shouldNotBecomeNegative |= sets[i].GetIndexOrNegative(id);
			}
			return shouldNotBecomeNegative;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int NegativeIfNotAssignedInAll(int id, SparseSet[] sets, int setsLength)
		{
			var shouldStayNegative = ~id;
			for (var i = 0; i < setsLength; i++)
			{
				shouldStayNegative &= sets[i].GetIndexOrNegative(id);
			}
			return shouldStayNegative;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static SparseSet GetMinimalSet(SparseSet[] sets)
		{
			var minimal = sets[0];
			for (var i = 1; i < sets.Length; i++)
			{
				if (minimal.Count > sets[i].Count)
				{
					minimal = sets[i];
				}
			}
			return minimal;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static SparseSet GetMinimalSet(SparseSet first, SparseSet[] sets)
		{
			var minimal = first;
			for (var i = 0; i < sets.Length; i++)
			{
				if (minimal.Count > sets[i].Count)
				{
					minimal = sets[i];
				}
			}
			return minimal;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static SparseSet GetMinimalSet(SparseSet set1, SparseSet set2)
		{
			if (set1.Count <= set2.Count)
			{
				return set1;
			}
			return set2;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static SparseSet GetMinimalSet(SparseSet set1, SparseSet set2, SparseSet set3)
		{
			if (set1.Count <= set2.Count && set1.Count <= set3.Count)
			{
				return set1;
			}
			if (set2.Count <= set1.Count && set2.Count <= set3.Count)
			{
				return set2;
			}
			return set3;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static SparseSet GetMinimalSet(SparseSet set1, SparseSet set2, SparseSet set3, SparseSet set4)
		{
			if (set1.Count <= set2.Count && set1.Count <= set3.Count && set1.Count <= set4.Count)
			{
				return set1;
			}
			if (set2.Count <= set1.Count && set2.Count <= set3.Count && set2.Count <= set4.Count)
			{
				return set2;
			}
			if (set3.Count <= set1.Count && set3.Count <= set2.Count && set3.Count <= set4.Count)
			{
				return set3;
			}
			return set4;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int GetOrderedHashCode(SparseSet[] orderedSets, Sets sets)
		{
			var hash = 17;
			for (var i = 0; i < orderedSets.Length; i++)
			{
				var index = sets.IndexOf(orderedSets[i]) + 1; // Avoid zero.
				hash = unchecked(hash * 31 + index);
			}

			return hash;
		}
	}
}
