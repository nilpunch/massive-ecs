using System;
using System.Runtime.InteropServices;
using Unity.IL2CPP.CompilerServices;

namespace Massive
{
	/// <summary>
	/// Defines the memory layout schema for pointer fields within a struct.<br/>
	/// Total available schema budget: <see cref="AllocatorDataSchema.Length"/> bytes<br/>
	/// Pointer field costs vary based on content type:<br/>
	/// <b>1 point</b>: Points to simple data (no nested pointers)<br/>
	/// <b>2 points</b>: Points to another pointer<br/>
	/// <b>3 points</b>: Points to a collection of pointers<br/>
	/// The total cost of all pointer fields must not exceed the available budget.
	/// </summary>
	[Il2CppSetOption(Option.NullChecks, false)]
	[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
	[StructLayout(LayoutKind.Sequential, Size = PreferedSize, Pack = PreferedSize)]
	public unsafe struct AllocatorDataSchema : IEquatable<AllocatorDataSchema>
	{
		public const int PreferedSize = 16;

		public const int Length = PreferedSize - 1;

		public const byte FlagMask = (byte.MaxValue >> 1) + 1;
		public const byte UsableMask = FlagMask - 1;

		public const byte MaxSchema = UsableMask;
		public const byte MaxOffset = UsableMask;
		public const byte MaxElementSize = byte.MaxValue;
		public const byte InvalidSchema = byte.MaxValue;

		/// <summary>
		/// PointerFieldOffset -(if MSB set)-> NestedShema -(if MSB set)-> CountFieldOffset.
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
