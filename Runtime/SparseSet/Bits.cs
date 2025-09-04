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
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Add(int id)
		{
			AddInternal(id);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Remove(int id)
		{
			RemoveInternal(id);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool Has(int id)
		{
			return HasInternal(id);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Bits And(BitsBase other)
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
		public Bits Not(BitsBase other)
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
		public Bits Or(BitsBase other)
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
