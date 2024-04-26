using System.Runtime.CompilerServices;
using Unity.IL2CPP.CompilerServices;

namespace Massive
{
	[Il2CppSetOption(Option.NullChecks, false)]
	[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
	public static class SetHelpers
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int CountAssignedInAll(int id, IReadOnlySet[] sets)
		{
			int counter = 0;
			foreach (var set in sets)
			{
				if (set.IsAssigned(id))
				{
					counter += 1;
				}
			}

			return counter;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool AssignedInAll(int id, IReadOnlySet[] sets)
		{
			foreach (var set in sets)
			{
				if (!set.IsAssigned(id))
				{
					return false;
				}
			}

			return true;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool NotAssignedInAll(int id, IReadOnlySet[] sets)
		{
			foreach (var set in sets)
			{
				if (set.IsAssigned(id))
				{
					return false;
				}
			}

			return true;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static IReadOnlySet GetMinimalSet(IReadOnlySet[] sets)
		{
			IReadOnlySet minimal = sets[0];

			for (int i = 1; i < sets.Length; i++)
			{
				if (minimal.Count > sets[i].Count)
				{
					minimal = sets[i];
				}
			}

			return minimal;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static IReadOnlySet GetMinimalSet(IReadOnlySet first, IReadOnlySet[] sets)
		{
			IReadOnlySet minimal = first;

			for (int i = 0; i < sets.Length; i++)
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
