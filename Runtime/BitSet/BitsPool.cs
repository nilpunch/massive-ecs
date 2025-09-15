using System;
using System.Runtime.CompilerServices;

namespace Massive
{
	public static class BitsPool
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
		public static BitSet RentClone(BitSetBase other)
		{
			var bits = Count > 0 ? Pool[--Count] : new BitSet();
			other.CopyBitsTo(bits);
			return bits;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void ReturnAndPop(BitSet bitSet)
		{
			if (Count >= Pool.Length)
			{
				Pool = Pool.Resize(MathUtils.NextPowerOf2(Count + 1));
			}

			Pool[Count++] = bitSet;
			bitSet.PopAll();
		}
	}
}
