using System.Runtime.CompilerServices;
using Unity.IL2CPP.CompilerServices;

namespace Massive
{
	[Il2CppSetOption(Option.NullChecks, false)]
	[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
	public static class SetHelpers
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int CountAliveInAll(int id, IReadOnlySet[] sets)
		{
			int counter = 0;
			foreach (var set in sets)
			{
				if (set.IsAlive(id))
				{
					counter += 1;
				}
			}

			return counter;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool AliveInAll(int id, IReadOnlySet[] sets)
		{
			foreach (var set in sets)
			{
				if (!set.IsAlive(id))
				{
					return false;
				}
			}

			return true;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool NotAliveInAll(int id, IReadOnlySet[] sets)
		{
			foreach (var set in sets)
			{
				if (set.IsAlive(id))
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
				if (minimal.AliveCount > sets[i].AliveCount)
				{
					minimal = sets[i];
				}
			}

			return minimal;
		}
	}
}