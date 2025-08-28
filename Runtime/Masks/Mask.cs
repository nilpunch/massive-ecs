using System;
using System.Runtime.CompilerServices;
using Unity.IL2CPP.CompilerServices;

namespace Massive
{
	[Il2CppSetOption(Option.NullChecks, false)]
	[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
	public struct Mask
	{
		public Masks Masks;

		public int FilledIncludeCount;
		public int[] FilledInclude;
		public long[] IncludeMask;

		public int FilledExcludeCount;
		public int[] FilledExclude;
		public long[] ExcludeMask;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Include(int componentId)
		{
			var maskIndex = componentId >> 6;
			if (maskIndex >= IncludeMask.Length)
			{
				IncludeMask = IncludeMask.Resize(MathUtils.NextPowerOf2(maskIndex + 1));
			}

			var bit = 1L << (componentId & 63);
			if ((IncludeMask[maskIndex] & bit) != 0)
			{
				return;
			}

			IncludeMask[maskIndex] |= bit;

			var fillIndex = ~Array.BinarySearch(FilledInclude, 0, FilledIncludeCount, maskIndex);
			if (fillIndex < 0)
			{
				return;
			}

			if (FilledIncludeCount >= FilledInclude.Length)
			{
				FilledInclude = FilledInclude.Resize(MathUtils.NextPowerOf2(FilledIncludeCount + 1));
			}

			Array.Copy(FilledInclude, fillIndex, FilledInclude, fillIndex + 1, FilledIncludeCount - fillIndex);
			FilledInclude[fillIndex] = maskIndex;
			FilledIncludeCount++;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Exclude(int componentId)
		{
			var maskIndex = componentId >> 6;
			if (maskIndex >= ExcludeMask.Length)
			{
				ExcludeMask = ExcludeMask.Resize(MathUtils.NextPowerOf2(maskIndex + 1));
			}

			var bit = 1L << (componentId & 63);
			if ((ExcludeMask[maskIndex] & bit) != 0)
			{
				return;
			}

			ExcludeMask[maskIndex] |= bit;

			var fillIndex = ~Array.BinarySearch(FilledExclude, 0, FilledExcludeCount, maskIndex);
			if (fillIndex < 0)
			{
				return;
			}

			if (FilledExcludeCount >= FilledExclude.Length)
			{
				FilledExclude = FilledExclude.Resize(MathUtils.NextPowerOf2(FilledExcludeCount + 1));
			}

			Array.Copy(FilledExclude, fillIndex, FilledExclude, fillIndex + 1, FilledExcludeCount - fillIndex);
			FilledExclude[fillIndex] = maskIndex;
			FilledExcludeCount++;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public readonly bool ContainsId(int id)
		{
			var shouldNotBecomeNegative = (long)id;
			var shouldStayNegative = (long)~id;
			var bitmap = Masks.BitMap;
			var maskOffset = id * Masks.MaskLength;

			for (var i = 0; i < FilledIncludeCount; i++)
			{
				var maskIndex = FilledInclude[i];
				shouldNotBecomeNegative |= (IncludeMask[maskIndex] & bitmap[maskOffset + maskIndex]) - 1;
			}

			for (var i = 0; i < FilledExcludeCount; i++)
			{
				var maskIndex = FilledExclude[i];
				shouldStayNegative &= (ExcludeMask[maskIndex] & bitmap[maskOffset + maskIndex]) - 1;
			}

			return (shouldNotBecomeNegative | ~shouldStayNegative) >= 0L;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Clear()
		{
			for (var i = 0; i < FilledIncludeCount; i++)
			{
				var maskIndex = FilledInclude[i];
				IncludeMask[maskIndex] = 0;
			}

			for (var i = 0; i < FilledExcludeCount; i++)
			{
				var maskIndex = FilledExclude[i];
				ExcludeMask[maskIndex] = 0;
			}

			FilledIncludeCount = 0;
			FilledExcludeCount = 0;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void CopyTo(ref Mask other)
		{
			other.Clear();

			if (FilledIncludeCount > other.FilledInclude.Length)
			{
				other.FilledInclude = new int[MathUtils.NextPowerOf2(FilledIncludeCount)];
			}
			if (FilledExcludeCount > other.FilledExclude.Length)
			{
				other.FilledExclude = new int[MathUtils.NextPowerOf2(FilledExcludeCount)];
			}

			if (FilledIncludeCount > 0)
			{
				var maxMaskIndex = FilledInclude[FilledIncludeCount - 1];
				if (maxMaskIndex >= other.IncludeMask.Length)
				{
					other.IncludeMask = other.IncludeMask.Resize(MathUtils.NextPowerOf2(maxMaskIndex + 1));
				}
			}

			if (FilledExcludeCount > 0)
			{
				var maxMaskIndex = FilledExclude[FilledExcludeCount - 1];
				if (maxMaskIndex >= other.ExcludeMask.Length)
				{
					other.ExcludeMask = other.ExcludeMask.Resize(MathUtils.NextPowerOf2(maxMaskIndex + 1));
				}
			}

			other.FilledIncludeCount = FilledIncludeCount;
			for (var i = 0; i < FilledIncludeCount; i++)
			{
				var idx = FilledInclude[i];
				other.FilledInclude[i] = idx;
				other.IncludeMask[idx] = IncludeMask[idx];
			}

			other.FilledExcludeCount = FilledExcludeCount;
			for (var i = 0; i < FilledExcludeCount; i++)
			{
				var idx = FilledExclude[i];
				other.FilledExclude[i] = idx;
				other.ExcludeMask[idx] = ExcludeMask[idx];
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public readonly void ReturnToPool()
		{
			Masks.ReturnMask(this);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Mask New(Masks masks)
		{
			return new Mask()
			{
				FilledInclude = Array.Empty<int>(),
				FilledExclude = Array.Empty<int>(),
				IncludeMask = Array.Empty<long>(),
				ExcludeMask = Array.Empty<long>(),
				Masks = masks,
			};
		}
	}
}
