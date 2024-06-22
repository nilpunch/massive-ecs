using System;
using System.Runtime.CompilerServices;
using Unity.IL2CPP.CompilerServices;

namespace Massive
{
	[Il2CppSetOption(Option.NullChecks, false)]
	[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
	public static class SetHelpers
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int CountAssignedInAll(int id, ArraySegment<IReadOnlySet> sets)
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
		public static bool AssignedInAll(int id, ArraySegment<IReadOnlySet> sets)
		{
			int shouldNotBecomeNegative = 0;
			for (int i = 0; i < sets.Count; i++)
			{
				shouldNotBecomeNegative |= sets[i].GetDenseOrInvalid(id);
			}

			return shouldNotBecomeNegative < 0;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool NotAssignedInAll(int id, ArraySegment<IReadOnlySet> sets)
		{
			int shouldStayNegative = ~0;
			for (int i = 0; i < sets.Count; i++)
			{
				shouldStayNegative &= sets[i].GetDenseOrInvalid(id);
			}

			return shouldStayNegative >= 0;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static IReadOnlySet GetMinimalSet(ArraySegment<IReadOnlySet> sets)
		{
			IReadOnlySet minimal = sets[0];

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
		public static IReadOnlySet GetMinimalSet(IReadOnlySet first, ArraySegment<IReadOnlySet> sets)
		{
			IReadOnlySet minimal = first;

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
		public static IReadOnlySet GetMinimalSet(IReadOnlySet set1, IReadOnlySet set2)
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
		public static IReadOnlySet GetMinimalSet(IReadOnlySet set1, IReadOnlySet set2, IReadOnlySet set3)
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
