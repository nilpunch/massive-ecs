using System;
using System.Runtime.CompilerServices;
using Unity.IL2CPP.CompilerServices;

namespace Massive
{
	[Il2CppSetOption(Option.NullChecks, false)]
	[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
	public class BitsBase
	{
		public ulong[] Bits0 { get; protected set; } = Array.Empty<ulong>();
		public ulong[] Bits1 { get; protected set; } = Array.Empty<ulong>();

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void GrowToFit(BitsBase other)
		{
			EnsureBits1Capacity(other.Bits1.Length);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void EnsureBits1Capacity(int bits1Capacity)
		{
			if (Bits1.Length < bits1Capacity)
			{
				Bits1 = Bits1.Resize(bits1Capacity);
				Bits0 = Bits0.Resize(bits1Capacity << 6);
			}
		}

		/// <summary>
		/// Copies bits from this set into the specified one.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void CopyBitsTo(BitsBase other)
		{
			other.GrowToFit(this);

			Array.Copy(Bits1, other.Bits1, Bits1.Length);
			Array.Copy(Bits0, other.Bits0, Bits0.Length);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static BitsBase GetMinBits(BitsBase[] bits)
		{
			var minimal = bits[0];
			for (var i = 1; i < bits.Length; i++)
			{
				if (minimal.Bits1.Length > bits[i].Bits1.Length)
				{
					minimal = bits[i];
				}
			}
			return minimal;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static BitsBase GetMinBits(SparseSet[] bits, int count)
		{
			var minimal = bits[0];
			for (var i = 1; i < count; i++)
			{
				if (minimal.Bits1.Length > bits[i].Bits1.Length)
				{
					minimal = bits[i];
				}
			}
			return minimal;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static BitsBase GetMinBits(BitsBase first, BitsBase[] bits)
		{
			var minimal = first;
			for (var i = 0; i < bits.Length; i++)
			{
				if (minimal.Bits1.Length > bits[i].Bits1.Length)
				{
					minimal = bits[i];
				}
			}
			return minimal;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static BitsBase GetMinBits(BitsBase first, SparseSet[] bits, int count)
		{
			var minimal = first;
			for (var i = 0; i < count; i++)
			{
				if (minimal.Bits1.Length > bits[i].Bits1.Length)
				{
					minimal = bits[i];
				}
			}
			return minimal;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static BitsBase GetMinBits(BitsBase bits1, BitsBase bits2)
		{
			if (bits1.Bits1.Length <= bits2.Bits1.Length)
			{
				return bits1;
			}
			return bits2;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static BitsBase GetMinBits(BitsBase bits1, BitsBase bits2, BitsBase bits3)
		{
			if (bits1.Bits1.Length <= bits2.Bits1.Length && bits1.Bits1.Length <= bits3.Bits1.Length)
			{
				return bits1;
			}
			if (bits2.Bits1.Length <= bits1.Bits1.Length && bits2.Bits1.Length <= bits3.Bits1.Length)
			{
				return bits2;
			}
			return bits3;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static BitsBase GetMinBits(BitsBase bits1, BitsBase bits2, BitsBase bits3, BitsBase bits4)
		{
			if (bits1.Bits1.Length <= bits2.Bits1.Length && bits1.Bits1.Length <= bits3.Bits1.Length && bits1.Bits1.Length <= bits4.Bits1.Length)
			{
				return bits1;
			}
			if (bits2.Bits1.Length <= bits1.Bits1.Length && bits2.Bits1.Length <= bits3.Bits1.Length && bits2.Bits1.Length <= bits4.Bits1.Length)
			{
				return bits2;
			}
			if (bits3.Bits1.Length <= bits1.Bits1.Length && bits3.Bits1.Length <= bits2.Bits1.Length && bits3.Bits1.Length <= bits4.Bits1.Length)
			{
				return bits3;
			}
			return bits4;
		}
	}
}
