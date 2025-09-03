using System;
using System.Runtime.CompilerServices;
using Unity.IL2CPP.CompilerServices;

namespace Massive
{
	[Il2CppSetOption(Option.NullChecks, false)]
	[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
	public class BitSet : BitSetBase
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Add(int id)
		{
			var id0 = id >> 6;
			var id1 = id >> 12;

			if (id1 >= Bits1.Length)
			{
				Bits1 = Bits1.Resize(MathUtils.NextPowerOf2(id1 + 1));
				Bits0 = Bits0.Resize(Bits1.Length << 6);
			}

			var bit0 = 1UL << (id & 63);
			var bit1 = 1UL << (id0 & 63);

			if (Bits0[id0] == 0UL)
			{
				Bits1[id1] |= bit1;
			}
			Bits0[id0] |= bit0;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Remove(int id)
		{
			var id0 = id >> 6;
			var id1 = id >> 12;

			if (id0 >= Bits0.Length)
			{
				return;
			}

			var bit0 = 1UL << (id & 63);
			var bit1 = 1UL << (id0 & 63);

			Bits0[id0] &= ~bit0;
			if (Bits0[id0] == 0UL)
			{
				Bits1[id1] &= ~bit1;
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public BitSet And(BitSetBase other)
		{
			var otherBits1Length = other.Bits1.Length;
			var otherBits0Length = otherBits1Length << 6;

			if (otherBits1Length > Bits1.Length)
			{
				Bits1 = Bits1.Resize(otherBits1Length);
				Bits0 = Bits0.Resize(otherBits0Length);
			}

			for (var i = 0; i < otherBits1Length; i++)
			{
				Bits1[i] &= other.Bits1[i];
			}

			for (var j = 0; j < otherBits0Length; j++)
			{
				Bits0[j] &= other.Bits0[j];
			}

			return this;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public BitSet Not(BitSetBase other)
		{
			var otherBits1Length = other.Bits1.Length;
			var otherBits0Length = otherBits1Length << 6;

			if (otherBits1Length > Bits1.Length)
			{
				Bits1 = Bits1.Resize(otherBits1Length);
				Bits0 = Bits0.Resize(otherBits0Length);
			}

			for (var i = 0; i < otherBits1Length; i++)
			{
				Bits1[i] &= ~other.Bits1[i];
			}

			for (var j = 0; j < otherBits0Length; j++)
			{
				Bits0[j] &= ~other.Bits0[j];
			}

			return this;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public BitSet Or(BitSetBase other)
		{
			var otherBits1Length = other.Bits1.Length;
			var otherBits0Length = otherBits1Length << 6;

			if (otherBits1Length > Bits1.Length)
			{
				Bits1 = Bits1.Resize(otherBits1Length);
				Bits0 = Bits0.Resize(otherBits0Length);
			}

			for (var i = 0; i < otherBits1Length; i++)
			{
				Bits1[i] |= other.Bits1[i];
			}

			for (var j = 0; j < otherBits0Length; j++)
			{
				Bits0[j] |= other.Bits0[j];
			}

			return this;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void CopyFrom(BitSetBase other)
		{
			var otherBits1Length = other.Bits1.Length;
			var otherBits0Length = otherBits1Length << 6;

			if (Bits1.Length < otherBits1Length)
			{
				Bits1 = Bits1.Resize(otherBits1Length);
				Bits0 = Bits0.Resize(otherBits0Length);
			}

			Array.Copy(other.Bits1, Bits1, otherBits1Length);
			Array.Copy(other.Bits0, Bits0, otherBits0Length);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void CopyTo(BitSet other)
		{
			var bits1Length = Bits1.Length;
			var bits0Length = bits1Length << 6;

			if (other.Bits1.Length < bits1Length)
			{
				other.Bits1 = other.Bits1.Resize(bits1Length);
				other.Bits0 = other.Bits0.Resize(bits0Length);
			}

			Array.Copy(Bits1, other.Bits1, bits1Length);
			Array.Copy(Bits0, other.Bits0, bits0Length);
		}
	}
}
