using System;
using System.Runtime.CompilerServices;

namespace Massive
{
	public static class MaskPool
	{
		private static Mask[] Pool { get; set; } = Array.Empty<Mask>();

		private static int Count { get; set; }

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Mask RentMask(this Components components)
		{
			if (Count > 0)
			{
				var mask = Pool[--Count];
				mask.Clear();
				mask.Components = components;
				return mask;
			}

			return Mask.New(components);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Mask RentMaskCopy(this Components components, Mask maskToCopy)
		{
			if (Count > 0)
			{
				var mask = Pool[--Count];
				maskToCopy.CopyTo(ref mask);
				mask.Components = components;
				return mask;
			}

			return Mask.New(components);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void ReturnMask(this Components _, Mask mask)
		{
			if (Count >= Pool.Length)
			{
				Pool = Pool.ResizeToNextPowOf2(Count + 1);
			}

			Pool[Count++] = mask;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void ReturnMask(Mask mask)
		{
			if (Count >= Pool.Length)
			{
				Pool = Pool.ResizeToNextPowOf2(Count + 1);
			}

			Pool[Count++] = mask;
		}
	}
}
