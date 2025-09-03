using System;
using System.Runtime.CompilerServices;

namespace Massive
{
	public static class BitSetPool
	{
		public static int Count { get; set; }

		public static BitSet[] Pool { get; set; } = Array.Empty<BitSet>();

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static BitSet Rent()
		{
			if (Count > 0)
			{
				return Pool[--Count];
			}

			return new BitSet();
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void Return(BitSet bitSetBase)
		{
			if (Count >= Pool.Length)
			{
				Pool = Pool.Resize(MathUtils.NextPowerOf2(Count + 1));
			}

			Pool[Count++] = bitSetBase;
		}
	}
}
