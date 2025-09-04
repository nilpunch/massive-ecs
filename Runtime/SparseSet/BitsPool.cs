using System;
using System.Runtime.CompilerServices;

namespace Massive
{
	public static class BitsPool
	{
		public static int Count { get; set; }

		public static Bits[] Pool { get; set; } = Array.Empty<Bits>();

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Bits Rent()
		{
			if (Count > 0)
			{
				return Pool[--Count];
			}

			return new Bits();
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Bits RentClone(BitsBase other)
		{
			Bits bits;
			if (Count > 0)
			{
				bits = Pool[--Count];
			}
			else
			{
				bits = new Bits();
			}
			other.CopyBitsTo(bits);
			return bits;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void Return(Bits bits)
		{
			if (Count >= Pool.Length)
			{
				Pool = Pool.Resize(MathUtils.NextPowerOf2(Count + 1));
			}

			Pool[Count++] = bits;
		}
	}
}
