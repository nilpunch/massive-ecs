using System.Runtime.CompilerServices;
using Unity.IL2CPP.CompilerServices;

namespace Massive
{
	[Il2CppEagerStaticClassConstruction]
	public static class MathUtils
	{
		/// <summary>
		/// Computes the smallest power of two greater than or equal to a value.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int NextPowerOf2(int value)
		{
			var v = (uint)value;

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
		/// Checks whether a value is a power of two or not.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool IsPowerOfTwo(int value)
		{
			return value > 0 && (value & (value - 1)) == 0;
		}

		/// <summary>
		/// Fast log2 for powers of two only.
		/// </summary>
		/// <param name="value"> Non-negative power of two value. </param>
		/// <remarks>
		/// Returns -1 when value is 0.
		/// </remarks>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int FastLog2(int value)
		{
			var lzc = value;
			lzc |= lzc >> 1;
			lzc |= lzc >> 2;
			lzc |= lzc >> 4;
			lzc |= lzc >> 8;
			lzc |= lzc >> 16;

			lzc -= lzc >> 1 & 0x55555555;
			lzc = (lzc >> 2 & 0x33333333) + (lzc & 0x33333333);
			lzc = (lzc >> 4) + lzc & 0x0f0f0f0f;
			lzc += lzc >> 8;
			lzc += lzc >> 16;

			// lzc = sizeof(int) * 8 - lzc & 0x0000003f;
			// log2 = sizeof(int) * 8 - lzc - 1;
			// So, log2 = (lzc & 0x0000003f) - 1;

			return (lzc & 0x0000003f) - 1;
		}

		/// <summary>
		/// Fast module for powers of two only.
		/// </summary>
		/// <param name="value"> Non-negative value. </param>
		/// <param name="mod"> Power of two value. </param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int FastMod(int value, int mod)
		{
			return value & (mod - 1);
		}

		/// <summary>
		/// Fast division for powers of two only.
		/// </summary>
		/// <param name="value"> Non-negative value. </param>
		/// <param name="powerOfTwo"> Power of divider. </param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int FastPowDiv(int value, int powerOfTwo)
		{
			return value >> powerOfTwo;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int LeadingZeroesCount(int x)
		{
			x |= x >> 1;
			x |= x >> 2;
			x |= x >> 4;
			x |= x >> 8;
			x |= x >> 16;

			x -= x >> 1 & 0x55555555;
			x = (x >> 2 & 0x33333333) + (x & 0x33333333);
			x = (x >> 4) + x & 0x0f0f0f0f;
			x += x >> 8;
			x += x >> 16;

			return sizeof(int) * 8 - (x & 0x0000003f);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int CombineHashes(int a, int b)
		{
			unchecked
			{
				var hash = 17;
				hash = hash * 31 + a;
				hash = hash * 31 + b;
				return hash;
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int SaturationAdd(int a, int b)
		{
			unchecked
			{
				var result = a + b;
				if (result < a)
				{
					return int.MaxValue;
				}
				return result;
			}
		}

		/// <summary>
		/// Increments and wraps the value to 1 instead of 0 on overflow.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void IncrementWrapTo1(ref uint x)
		{
			unchecked
			{
				++x;
				if (x == 0)
				{
					x = 1;
				}
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int Max(int a, int b)
		{
			return a > b ? a : b;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int Min(int a, int b)
		{
			return a < b ? a : b;
		}
	}
}
