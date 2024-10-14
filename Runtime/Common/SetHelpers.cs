using System;
using System.Runtime.CompilerServices;
using Unity.IL2CPP.CompilerServices;

namespace Massive
{
	[Il2CppSetOption(Option.NullChecks | Option.ArrayBoundsChecks, false)]
	public static class SetHelpers
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int CountAssignedInAll(int id, ArraySegment<SparseSet> sets)
		{
			int counter = 0;
			for (var i = 0; i < sets.Count; i++)
			{
				var set = sets[i];
				if (set.IsAssigned(id))
				{
					counter += 1;
				}
			}

			return counter;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool AssignedInAll(int id, ArraySegment<SparseSet> sets)
		{
			int shouldNotBecomeNegative = 0;
			for (int i = 0; i < sets.Count; i++)
			{
				shouldNotBecomeNegative |= sets[i].GetIndexOrInvalid(id);
			}

			return shouldNotBecomeNegative >= 0;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool NotAssignedInAll(int id, ArraySegment<SparseSet> sets)
		{
			int shouldStayNegative = ~0;
			for (int i = 0; i < sets.Count; i++)
			{
				shouldStayNegative &= sets[i].GetIndexOrInvalid(id);
			}

			return shouldStayNegative < 0;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static SparseSet GetMinimalSet(ArraySegment<SparseSet> sets)
		{
			SparseSet minimal = sets[0];

			for (int i = 1; i < sets.Count; i++)
			{
				if (minimal.Count > sets[i].Count)
				{
					minimal = sets[i];
				}
			}

			return minimal;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static SparseSet GetMinimalSet(SparseSet first, ArraySegment<SparseSet> sets)
		{
			SparseSet minimal = first;

			for (int i = 0; i < sets.Count; i++)
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
			else
			{
				return set2;
			}
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
	}
}
