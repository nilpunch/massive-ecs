#if !MASSIVE_DISABLE_ASSERT
#define MASSIVE_ASSERT
#endif

using System;
using System.Runtime.CompilerServices;
using Unity.IL2CPP.CompilerServices;

namespace Massive
{
	[Il2CppSetOption(Option.NullChecks, false)]
	[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
	public struct Mask
	{
		public Components Components;

		public int CommonMaskCount;

		public ulong[] IncludeMask;

		public ulong[] ExcludeMask;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Include(SparseSet set)
		{
			var componentId = set.ComponentId;

			var maskIndex = componentId >> 6;
			if (maskIndex >= IncludeMask.Length)
			{
				IncludeMask = IncludeMask.ResizeToNextPowOf2(maskIndex + 1);
				ExcludeMask = ExcludeMask.ResizeToNextPowOf2(maskIndex + 1);
			}

			CommonMaskCount = MathUtils.Max(CommonMaskCount, maskIndex + 1);

			var bit = 1UL << (componentId & 63);
			if ((IncludeMask[maskIndex] & bit) != 0)
			{
				return;
			}

			IncludeMask[maskIndex] |= bit;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Exclude(SparseSet set)
		{
			var componentId = set.ComponentId;

			var maskIndex = componentId >> 6;
			if (maskIndex >= ExcludeMask.Length)
			{
				IncludeMask = IncludeMask.ResizeToNextPowOf2(maskIndex + 1);
				ExcludeMask = ExcludeMask.ResizeToNextPowOf2(maskIndex + 1);
			}

			CommonMaskCount = MathUtils.Max(CommonMaskCount, maskIndex + 1);

			var bit = 1UL << (componentId & 63);
			if ((ExcludeMask[maskIndex] & bit) != 0)
			{
				return;
			}

			ExcludeMask[maskIndex] |= bit;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public readonly bool ContainsId(int id)
		{
			var bitmap = Components.BitMap;
			var maskLength = Components.MaskLength;
			var offset = id * maskLength;

			var includeMask = IncludeMask;
			var excludeMask = ExcludeMask;

			var shouldStayZero = 0UL;

			for (var i = 0; i < CommonMaskCount; i++)
			{
				shouldStayZero = shouldStayZero
					| includeMask[i] & ~bitmap[offset + i]
					| excludeMask[i] & bitmap[offset + i];
			}

			return shouldStayZero == 0;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Clear()
		{
			for (var i = 0; i < CommonMaskCount; i++)
			{
				IncludeMask[i] = 0UL;
				ExcludeMask[i] = 0UL;
			}

			CommonMaskCount = 0;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public readonly void CopyTo(ref Mask other)
		{
			other.Clear();
			
			if (IncludeMask.Length > other.IncludeMask.Length)
			{
				other.IncludeMask = other.IncludeMask.Resize(IncludeMask.Length);
				other.ExcludeMask = other.ExcludeMask.Resize(IncludeMask.Length);
			}

			other.CommonMaskCount = CommonMaskCount;

			for (var i = 0; i < CommonMaskCount; i++)
			{
				other.IncludeMask[i] = IncludeMask[i];
				other.ExcludeMask[i] = ExcludeMask[i];
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public readonly void ReturnToPool()
		{
			Components.ReturnMask(this);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Mask New(Components components)
		{
			return new Mask()
			{
				CommonMaskCount = 0,
				IncludeMask = Array.Empty<ulong>(),
				ExcludeMask = Array.Empty<ulong>(),
				Components = components,
			};
		}
	}
}
