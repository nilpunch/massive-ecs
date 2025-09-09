using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.IL2CPP.CompilerServices;

namespace Massive
{
	[Il2CppSetOption(Option.NullChecks, false)]
	[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
	public class Bits : BitsBase
	{
		private List<IBitsBacktrack> PopOnAdd { get; } = new List<IBitsBacktrack>();
		private List<IBitsBacktrack> PopOnRemove { get; } = new List<IBitsBacktrack>();

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Bits RemoveOnAdd(IBitsBacktrack bits)
		{
			bits.PushRemoveOnAdd(this);
			PopOnAdd.Add(bits);
			return this;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Bits RemoveOnRemove(IBitsBacktrack bits)
		{
			bits.PushRemoveOnRemove(this);
			PopOnRemove.Add(bits);
			return this;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void PopAll()
		{
			foreach (var bitSet in PopOnAdd)
			{
				bitSet.PopRemoveOnAdd();
			}
			foreach (var bitSet in PopOnRemove)
			{
				bitSet.PopRemoveOnRemove();
			}
			PopOnAdd.Clear();
			PopOnRemove.Clear();
		}

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
		public bool Has(int id)
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
		public Bits AndBits(BitsBase other)
		{
			var minBits = GetMinBits(this, other);

			if (Bits1.Length > other.Bits1.Length)
			{
				Array.Fill(Bits1, 0UL, other.Bits1.Length, Bits1.Length - other.Bits1.Length);
				Array.Fill(Bits0, 0UL, other.Bits0.Length, Bits0.Length - other.Bits0.Length);
			}

			for (var i = 0; i < minBits.Bits1.Length; i++)
			{
				Bits1[i] &= other.Bits1[i];
			}

			for (var j = 0; j < minBits.Bits0.Length; j++)
			{
				Bits0[j] &= other.Bits0[j];
			}

			return this;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Bits NotBits(BitsBase other)
		{
			var minBits = GetMinBits(this, other);

			for (var i = 0; i < minBits.Bits1.Length; i++)
			{
				Bits1[i] &= ~other.Bits1[i];
			}

			for (var j = 0; j < minBits.Bits0.Length; j++)
			{
				Bits0[j] &= ~other.Bits0[j];
			}

			return this;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Bits OrBits(BitsBase other)
		{
			GrowToFit(other);

			for (var i = 0; i < other.Bits1.Length; i++)
			{
				Bits1[i] |= other.Bits1[i];
			}

			for (var j = 0; j < other.Bits0.Length; j++)
			{
				Bits0[j] |= other.Bits0[j];
			}

			return this;
		}
	}
}
