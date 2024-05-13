using System.Runtime.CompilerServices;

namespace Massive
{
	public static class MathHelpers
	{
		/// <summary>
		/// Computes the smallest power of two greater than or equal to a value.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int GetNextPowerOf2(int value)
		{
			uint v = (uint)value;

			if (v == 0)
			{
				return 0;
			}

			v--;
			v |= v >> 1;
			v |= v >> 2;
			v |= v >> 4;
			v |= v >> 8;
			v |= v >> 16;
			v++;

			return (int)v;
		}

		/// <summary>
		/// Fast module for powers of two only.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool IsPowerOfTwo(int value)
		{
			uint v = (uint)value;
			return v != 0 && (v & (v - 1)) == 0;
		}

		/// <summary>
		/// Fast module for powers of two only.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int FastMod(int value, int mod)
		{
			uint v = (uint)value;
			uint m = (uint)mod;
			return (int)(v & (m - 1));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int CombineHashes(int a, int b)
		{
			unchecked
			{
				int hash = 17;
				hash = hash * 31 + a;
				hash = hash * 31 + b;
				return hash;
			}
		}
	}
}
