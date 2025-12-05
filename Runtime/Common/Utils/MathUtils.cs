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

		private static readonly byte[] LogTable256;

		static MathUtils()
		{
			LogTable256 = new byte[256];
			LogTable256[0] = LogTable256[1] = 0;
			for (var i = 2; i < 256; i++)
			{
				LogTable256[i] = (byte)(1 + LogTable256[i / 2]);
			}
		}

		/// <remarks>
		/// Returns 0 when value is 0.
		/// </remarks>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int RoundUpToPowerOfTwo(int value)
		{
			var v = (uint)value;

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
			return (value & (value - 1)) == 0;
		}

		/// <remarks>
		/// Returns 0 when value is 0.
		/// </remarks>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int FloorLog2(uint x)
		{
			uint t;

			if ((t = x >> 24) > 0)
			{
				return 24 + LogTable256[t];
			}
			else if ((t = x >> 16) > 0)
			{
				return 16 + LogTable256[t];
			}
			else if ((t = x >> 8) > 0)
			{
				return 8 + LogTable256[t];
			}
			else
			{
				return LogTable256[x];
			}
		}

		/// <remarks>
		/// Returns 32 when value is 0.
		/// </remarks>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int CeilLog2(uint x)
		{
			uint t;
			x--;

			if ((t = x >> 24) > 0)
			{
				return 25 + LogTable256[t];
			}
			else if ((t = x >> 16) > 0)
			{
				return 17 + LogTable256[t];
			}
			else if ((t = x >> 8) > 0)
			{
				return 9 + LogTable256[t];
			}
			else
			{
				return 1 + LogTable256[x];
			}
		}

		/// <remarks>
		/// Returns 0 when value is 0.
		/// </remarks>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int FloorLog2(ulong x)
		{
			return MSB(x);
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
		public static int PopCount(uint x)
		{
			x -= (x >> 1) & 0x55555555U;
			x = (x & 0x33333333U) + ((x >> 2) & 0x33333333U);
			x = (x + (x >> 4)) & 0x0F0F0F0FU;
			return (int)((x * 0x01010101U) >> 24);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int LSB(ulong x)
		{
			return DeBruijn[(int)(((x & (ulong)-(long)x) * 0x37E84A99DAE458FUL) >> 58)];
		}

		/// <remarks>
		/// Returns 0 when value is 0.
		/// </remarks>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int MSB(ulong x)
		{
			const ulong MSB32 = 1UL << 32;

			var baseLog = 0;
			ulong t;
			if (x >= MSB32)
			{
				baseLog = 32;
				x >>= 32;
			}

			if ((t = x >> 24) > 0)
			{
				return baseLog + 24 + LogTable256[(int)t];
			}
			else if ((t = x >> 16) > 0)
			{
				return baseLog + 16 + LogTable256[(int)t];
			}
			else if ((t = x >> 8) > 0)
			{
				return baseLog + 8 + LogTable256[(int)t];
			}
			else
			{
				return baseLog + LogTable256[(int)x];
			}
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
			if (b > a)
			{
				a = b;
			}

			return a;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int Max(int a, int b, int c)
		{
			if (b > a)
			{
				a = b;
			}

			if (c > a)
			{
				a = c;
			}

			return a;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int Min(int a, int b)
		{
			if (b < a)
			{
				a = b;
			}

			return a;
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
