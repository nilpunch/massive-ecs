using System;
using System.Runtime.CompilerServices;
using Unity.IL2CPP.CompilerServices;

namespace Massive
{
	[Il2CppSetOption(Option.NullChecks, false)]
	[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
	public static class ViewUtils
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool AliveInAll(int id, ISet[] sets)
		{
			for (int i = 0; i < sets.Length; i++)
			{
				if (!sets[i].IsAlive(id))
				{
					return false;
				}
			}

			return true;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool NotAliveInAll(int id, ISet[] sets)
		{
			for (int i = 0; i < sets.Length; i++)
			{
				if (sets[i].IsAlive(id))
				{
					return false;
				}
			}

			return true;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ISet GetMinimalSet(ISet[] sets)
		{
			ISet minimal = sets[0];

			for (int i = 1; i < sets.Length; i++)
			{
				if (minimal.AliveCount > sets[i].AliveCount)
				{
					minimal = sets[i];
				}
			}

			return minimal;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool TryGetAllDense(int id, ISet[] sets, Span<int> allDense)
		{
			for (int i = 0; i < sets.Length; i++)
			{
				if (sets[i].TryGetDense(id, out var dense))
				{
					allDense[i] = dense;
				}
				else
				{
					return false;
				}
			}

			return true;
		}
	}
}