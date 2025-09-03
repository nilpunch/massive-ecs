using System;
using System.Runtime.CompilerServices;
using Unity.IL2CPP.CompilerServices;

namespace Massive
{
	[Il2CppSetOption(Option.NullChecks, false)]
	[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
	public class BitSetBase
	{
		public ulong[] Bits0 { get; protected set; } = Array.Empty<ulong>();
		public ulong[] Bits1 { get; protected set; } = Array.Empty<ulong>();

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool Has(int id)
		{
			var id0 = id >> 6;

			if (id0 >= Bits0.Length)
			{
				return false;
			}

			var bit0 = 1UL << (id & 63);
			return (Bits0[id0] & bit0) != 0UL;
		}

		public static BitSetBase GetMinBitSet(BitSetBase set1, BitSetBase set2, BitSetBase set3, BitSetBase set4)
		{
			if (set1.Bits1.Length <= set2.Bits1.Length && set1.Bits1.Length <= set3.Bits1.Length && set1.Bits1.Length <= set4.Bits1.Length)
			{
				return set1;
			}
			if (set2.Bits1.Length <= set1.Bits1.Length && set2.Bits1.Length <= set3.Bits1.Length && set2.Bits1.Length <= set4.Bits1.Length)
			{
				return set2;
			}
			if (set3.Bits1.Length <= set1.Bits1.Length && set3.Bits1.Length <= set2.Bits1.Length && set3.Bits1.Length <= set4.Bits1.Length)
			{
				return set3;
			}
			return set4;
		}
	}
}
