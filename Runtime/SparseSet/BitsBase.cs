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

		public Bits[] RemoveOnAdd { get; private set; } = Array.Empty<Bits>();
		public int RemoveOnAddCount { get; private set; }

		public Bits[] RemoveOnRemove { get; private set; } = Array.Empty<Bits>();
		public int RemoveOnRemoveCount { get; private set; }

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void GrowToFit(BitsBase other)
		{
			if (Bits1.Length < other.Bits1.Length)
			{
				Bits1 = Bits1.Resize(other.Bits1.Length);
				Bits0 = Bits0.Resize(other.Bits0.Length);
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
		public void PushRemoveOnAdd(Bits bits)
		{
			if (RemoveOnAddCount >= RemoveOnAdd.Length)
			{
				RemoveOnAdd = RemoveOnAdd.ResizeToNextPowOf2(RemoveOnAddCount + 1);
			}

			RemoveOnAdd[RemoveOnAddCount++] = bits;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void PopRemoveOnAdd()
		{
			RemoveOnAddCount--;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void PushRemoveOnRemove(Bits bits)
		{
			if (RemoveOnRemoveCount >= RemoveOnRemove.Length)
			{
				RemoveOnRemove = RemoveOnRemove.ResizeToNextPowOf2(RemoveOnRemoveCount + 1);
			}

			RemoveOnRemove[RemoveOnRemoveCount++] = bits;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void PopRemoveOnRemove()
		{
			RemoveOnRemoveCount--;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		protected void AddInternal(int id)
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

			for (var i = 0; i < RemoveOnAddCount; i++)
			{
				RemoveOnAdd[i].Remove(id);
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		protected void RemoveInternal(int id)
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

			for (var i = 0; i < RemoveOnRemoveCount; i++)
			{
				RemoveOnRemove[i].Remove(id);
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		protected bool HasInternal(int id)
		{
			var id0 = id >> 6;

			if (id < 0 || id0 >= Bits0.Length)
			{
				return false;
			}

			var bit0 = 1UL << (id & 63);
			return (Bits0[id0] & bit0) != 0UL;
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
		public static BitsBase GetMinBits(BitSet[] bits, int count)
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
		public static BitsBase GetMinBits(BitsBase first, BitSet[] bits, int count)
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
