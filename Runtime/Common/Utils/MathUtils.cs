using System.Runtime.CompilerServices;
using Unity.IL2CPP.CompilerServices;

namespace Massive
{
	[Il2CppEagerStaticClassConstruction]
	public static class MathUtils
	{
		public static readonly byte[] DeBruijn =
		{
			0, 1, 17, 2, 18, 50, 3, 57, 47, 19, 22, 51, 29, 4, 33, 58,
			15, 48, 20, 27, 25, 23, 52, 41, 54, 30, 38, 5, 43, 34, 59, 8,
			63, 16, 49, 56, 46, 21, 28, 32, 14, 26, 24, 40, 53, 37, 42, 7,
			62, 55, 45, 31, 13, 39, 36, 6, 61, 44, 12, 35, 60, 11, 10, 9,
		};

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

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void NextPowerOf2AndLog2(int value, out int nextPowerOf2, out int log2)
		{
			var v = (uint)value;
    
			if (v == 0)
			{
				nextPowerOf2 = 0;
				log2 = -1;
				return; // log2(0) is undefined, return -1 as per your convention
			}

			// Calculate next power of 2
			v--;
			v |= v >> 1;
			v |= v >> 2;
			v |= v >> 4;
			v |= v >> 8;
			v |= v >> 16;
			v++;
    
			nextPowerOf2 = (int)v;
    
			// Calculate log2 using the same bit pattern we already computed
			// For a power of 2, v-1 gives us the mask we need for population count
			uint lzc = v - 1;
			lzc -= lzc >> 1 & 0x55555555;
			lzc = (lzc >> 2 & 0x33333333) + (lzc & 0x33333333);
			lzc = (lzc >> 4) + lzc & 0x0f0f0f0f;
			lzc += lzc >> 8;
			lzc += lzc >> 16;
    
			log2 = (int)(lzc & 0x0000003f);
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
		/// <param name="divider"> Power of divider. </param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int FastDiv(int value, int divider)
		{
			return value >> divider;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int PopCount(ulong x)
		{
			x -= (x >> 1) & 0x5555555555555555UL;
			x = (x & 0x3333333333333333UL) + ((x >> 2) & 0x3333333333333333UL);
			x = (x + (x >> 4)) & 0x0F0F0F0F0F0F0F0FUL;
			return (int)((x * 0x0101010101010101UL) >> 56);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int LSB(ulong x)
		{
			return DeBruijn[(int)(((x & (ulong)-(long)x) * 0x37E84A99DAE458FUL) >> 58)];
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int ApproximateMSB(ulong x)
		{
			const ulong MSB48 = 1UL << 48;
			const ulong MSB32 = 1UL << 32;
			const ulong MSB16 = 1UL << 16;

			if (x >= MSB32)
			{
				return x >= MSB48 ? 64 : 48;
			}

			return x >= MSB16 ? 32 : 16;
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

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int AlignmentPadding(int offset, int alignment)
		{
			return -offset & (alignment - 1);
		}
		
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static long AlignmentPadding(long offset, int alignment)
		{
			return -offset & (alignment - 1);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int Aligned(int offset, int alignment)
		{
			return (offset + (alignment - 1)) & -alignment;
		}
	}
}
