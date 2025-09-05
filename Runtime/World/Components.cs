#if !MASSIVE_DISABLE_ASSERT
#define MASSIVE_ASSERT
#endif

using System;
using System.Runtime.CompilerServices;
using Unity.IL2CPP.CompilerServices;

namespace Massive
{
	/// <summary>
	/// Acceleration structure for managing components.
	/// </summary>
	[Il2CppSetOption(Option.NullChecks, false)]
	[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
	public class Components
	{
		private readonly byte[] DeBruijn =
		{
			0, 1, 17, 2, 18, 50, 3, 57, 47, 19, 22, 51, 29, 4, 33, 58,
			15, 48, 20, 27, 25, 23, 52, 41, 54, 30, 38, 5, 43, 34, 59, 8,
			63, 16, 49, 56, 46, 21, 28, 32, 14, 26, 24, 40, 53, 37, 42, 7,
			62, 55, 45, 31, 13, 39, 36, 6, 61, 44, 12, 35, 60, 11, 10, 9,
		};

		public long[] BitMap { get; private set; } = Array.Empty<long>();

		/// <summary>
		/// Has capacity of MaskLenght * 64.
		/// </summary>
		public int[] Buffer { get; private set; } = Array.Empty<int>();

		public int BitMapCapacity { get; private set; }

		public int MaskLength { get; private set; }

		public int EntitiesCapacity { get; private set; }

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Set(int entityId, int componentId)
		{
			var index = componentId >> 6; // div 64.
			var bit = componentId & 63; // mod 64.
			BitMap[entityId * MaskLength + index] |= 1L << bit;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Remove(int entityId, int componentId)
		{
			var index = componentId >> 6; // div 64.
			var bit = componentId & 63; // mod 64.
			BitMap[entityId * MaskLength + index] &= ~(1L << bit);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool Has(int entityId, int componentId)
		{
			var index = componentId >> 6; // div 64.
			var bit = componentId & 63; // mod 64.
			return (BitMap[entityId * MaskLength + index] & (1L << bit)) != 0;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public int GetAll(int entityId, int[] buffer)
		{
			var componentCount = 0;
			var maskIndex = entityId * MaskLength;
			for (var i = 0; i < MaskLength; i++, maskIndex++)
			{
				var componentOffset = i << 6; // mul 64.
				var mask = BitMap[maskIndex];

				// Algorithm adapted from StaticEcs
				// Source: https://github.com/Felid-Force-Studios/StaticEcs/blob/be8bb1c668309294aeecef80313677da368d7703/Src/Utils/BitMask.cs#L432
				while (mask != 0L)
				{
					var componentIndex = DeBruijn[((ulong)(mask & -mask) * 0x37E84A99DAE458F) >> 58]; // LSB(v).
					buffer[componentCount++] = componentOffset + componentIndex;
					mask &= mask - 1L;
				}
			}

			return componentCount;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public int GetAllAndRemove(int entityId, int[] buffer)
		{
			var componentCount = 0;
			var maskIndex = entityId * MaskLength;
			for (var i = 0; i < MaskLength; i++, maskIndex++)
			{
				var componentOffset = i << 6; // mul 64.
				var mask = BitMap[maskIndex];

				while (mask != 0L)
				{
					var componentIndex = DeBruijn[((ulong)(mask & -mask) * 0x37E84A99DAE458F) >> 58]; // LSB(v).
					buffer[componentCount++] = componentOffset + componentIndex;
					mask &= mask - 1L;
				}

				BitMap[maskIndex] = 0L;
			}

			return componentCount;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void EnsureEntitiesCapacity(int capacity)
		{
			if (capacity > EntitiesCapacity)
			{
				EntitiesCapacity = MathUtils.NextPowerOf2(capacity);
				BitMapCapacity = MaskLength * EntitiesCapacity;
				BitMap = BitMap.Resize(BitMapCapacity);
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void EnsureComponentsCapacity(int capacity)
		{
			var maskLength = (capacity + 63) >> 6;

			if (maskLength > MaskLength)
			{
				var oldMaskLength = MaskLength;
				var newBitMap = new long[maskLength * EntitiesCapacity];

				for (var entityId = 0; entityId < EntitiesCapacity; entityId++)
				{
					var oldOffset = entityId * oldMaskLength;
					var newOffset = entityId * maskLength;

					for (var i = 0; i < oldMaskLength; i++)
					{
						newBitMap[newOffset + i] = BitMap[oldOffset + i];
					}
				}

				BitMap = newBitMap;
				MaskLength = maskLength;
				BitMapCapacity = BitMap.Length;
				Buffer = Buffer.Resize(maskLength * 64);
			}
		}

		/// <summary>
		/// Creates and returns a new sparse set that is an exact copy of this one.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Components Clone()
		{
			var clone = new Components();
			CopyTo(clone);
			return clone;
		}

		/// <summary>
		/// Copies all sparse state from this set into the specified one.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void CopyTo(Components other)
		{
			var bitmapCapacity = BitMapCapacity;
			var otherBitmapCapacity = other.BitMapCapacity;

			if (other.MaskLength != MaskLength)
			{
				other.Buffer = other.Buffer.Resize(MaskLength * 64);
				other.MaskLength = MaskLength;
				other.EntitiesCapacity = otherBitmapCapacity / MaskLength;
			}

			if (otherBitmapCapacity < bitmapCapacity)
			{
				other.BitMap = new long[bitmapCapacity];
				other.EntitiesCapacity = EntitiesCapacity;
				other.BitMapCapacity = bitmapCapacity;
			}

			Array.Copy(BitMap, other.BitMap, bitmapCapacity);

			if (bitmapCapacity < otherBitmapCapacity)
			{
				Array.Fill(other.BitMap, 0L, bitmapCapacity, otherBitmapCapacity - bitmapCapacity);
			}
		}
	}
}
