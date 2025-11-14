using System;
using System.Runtime.InteropServices;
using Unity.IL2CPP.CompilerServices;

namespace Massive
{
	[Il2CppSetOption(Option.NullChecks, false)]
	[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
	[StructLayout(LayoutKind.Sequential, Size = PreferedSize, Pack = PreferedSize)]
	public unsafe struct AllocatorDataSchema : IEquatable<AllocatorDataSchema>
	{
		public const int PreferedSize = 8;

		public const int Length = PreferedSize - 1;
		public const byte MaxSchema = byte.MaxValue >> 1;
		public const byte FlagMask = 1 << 7;
		public const byte UsableMask = FlagMask - 1;

		/// <summary>
		/// PointerFieldOffset -(MSB set)-> NestedShema -(MSB set)-> CountFieldOffset.
		/// </summary>
		public fixed byte OffsetSchemaCount[Length];

		public byte ElementSize;

		public bool Equals(AllocatorDataSchema other)
		{
			if (ElementSize != other.ElementSize)
			{
				return false;
			}

			var isNotEqual = false;
			for (var i = 0; i < Length; i++)
			{
				isNotEqual |= OffsetSchemaCount[i] != other.OffsetSchemaCount[i];
			}

			return !isNotEqual;
		}

		public override bool Equals(object obj)
		{
			return obj is AllocatorDataSchema other && Equals(other);
		}

		public override int GetHashCode()
		{
			var combined = (int)ElementSize;
			for (var i = 0; i < Length; i++)
			{
				combined = HashCode.Combine(combined, OffsetSchemaCount[i]);
			}

			return combined;
		}
	}
}
