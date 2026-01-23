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
		// Storage adapted from StaticEcs.
		// Source: https://github.com/Felid-Force-Studios/StaticEcs/blob/be8bb1c668309294aeecef80313677da368d7703/Src/Utils/BitMask.cs

		private byte[] DeBruijn { get; } = MathUtils.DeBruijn;

		public ulong[] BitMap { get; private set; } = Array.Empty<ulong>();

		public int BitMapCapacity { get; private set; }

		/// <summary>
		/// Has capacity of MaskLenght * 64.
		/// </summary>
		public int[] Buffer { get; private set; } = new int[64];

		public int MaskLength { get; private set; } = 1;

		public int EntitiesCapacity { get; private set; }

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Set(int entityId, int componentId)
		{
			var index = componentId >> 6;
			var bit = componentId & 63;
			BitMap[entityId * MaskLength + index] |= 1UL << bit;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Remove(int entityId, int componentId)
		{
			var index = componentId >> 6;
			var bit = componentId & 63;
			BitMap[entityId * MaskLength + index] &= ~(1UL << bit);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool Has(int entityId, int componentId)
		{
			var index = componentId >> 6;
			var bit = componentId & 63;
			return (BitMap[entityId * MaskLength + index] & (1UL << bit)) != 0;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public int GetAll(int entityId, int[] buffer)
		{
			var componentCount = 0;
			var maskIndex = entityId * MaskLength;
			for (var i = 0; i < MaskLength; i++, maskIndex++)
			{
				var componentOffset = i << 6;
				var mask = BitMap[maskIndex];

				while (mask != 0UL)
				{
					var componentIndex = (int)DeBruijn[(int)(((mask & (ulong)-(long)mask) * 0x37E84A99DAE458FUL) >> 58)];
					buffer[componentCount++] = componentOffset + componentIndex;
					mask &= mask - 1UL;
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
				var componentOffset = i << 6;
				var mask = BitMap[maskIndex];

				while (mask != 0L)
				{
					var componentIndex = (int)DeBruijn[(int)(((mask & (ulong)-(long)mask) * 0x37E84A99DAE458FUL) >> 58)];
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
				EntitiesCapacity = MathUtils.RoundUpToPowerOfTwo(capacity);
				var requiredCapacity = MaskLength * EntitiesCapacity;
				if (requiredCapacity > BitMapCapacity)
				{
					BitMapCapacity = requiredCapacity;
					BitMap = BitMap.Resize(BitMapCapacity);
				}
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void EnsureComponentsCapacity(int capacity)
		{
			var maskLength = (capacity >> 6) + 1;

			if (maskLength > MaskLength)
			{
				var oldMaskLength = MaskLength;
				var newBitMap = new ulong[maskLength * EntitiesCapacity];

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
				EnsureBufferMaskLength();
			}
		}

		/// <summary>
		/// Does not preserve bitmap data.
		/// </summary>
		public void EnsureBitMapCapacity(int entitiesCapacity, int maskLength)
		{
			var bitmapCapacity = entitiesCapacity * maskLength;

			if (maskLength != MaskLength)
			{
				MaskLength = maskLength;
				EnsureBufferMaskLength();
			}

			if (BitMapCapacity < bitmapCapacity)
			{
				BitMap = new ulong[bitmapCapacity];
				BitMapCapacity = bitmapCapacity;
			}

			EntitiesCapacity = BitMapCapacity / maskLength;
		}

		private void EnsureBufferMaskLength()
		{
			if (MaskLength << 6 > Buffer.Length)
			{
				Buffer = Buffer.Resize(MaskLength << 6);
			}
		}

		public void Reset()
		{
			Array.Fill(BitMap, 0UL);
			MaskLength = 1;
			EntitiesCapacity = BitMapCapacity / MaskLength;
		}

		public Components Clone()
		{
			var clone = new Components();
			CopyTo(clone);
			return clone;
		}

		public void CopyTo(Components other)
		{
			other.EnsureBitMapCapacity(EntitiesCapacity, MaskLength);

			Array.Copy(BitMap, other.BitMap, BitMapCapacity);

			if (BitMapCapacity < other.BitMapCapacity)
			{
				Array.Fill(other.BitMap, 0UL, BitMapCapacity, other.BitMapCapacity - BitMapCapacity);
			}
		}
	}
}
