using Unity.IL2CPP.CompilerServices;

namespace Massive
{
	[Il2CppEagerStaticClassConstruction]
	[Il2CppSetOption(Option.NullChecks, false)]
	[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
	public static class Constants
	{
		public const string LibraryName = "Massive ECS";

		public const int DefaultFramesCapacity = 121;

		public const int InvalidId = -1;

		// Must be power of 2 in range [64, 4096].
		public const int PageSize = 256;
		public const int PageSizePower = 8;
		public const int PageSizeMinusOne = PageSize - 1;
		public const ulong BasePageMask = ~0UL >> (64 - (PageSize >> 6));
		public const int PagesInBlockMinusOne = ((1 << 12) >> PageSizePower) - 1;
		public const int PageMaskShift = PageSizePower - 6;
		public const int PagesInBlockPower = 12 - PageSizePower;

		public static readonly ulong[] PageMasks;
		public static readonly ulong[] PageMasksNegative;

		static Constants()
		{
			const int pagesInBlock = PagesInBlockMinusOne + 1;
			PageMasks = new ulong[pagesInBlock];
			PageMasksNegative = new ulong[pagesInBlock];

			for (var i = 0; i < pagesInBlock; i++)
			{
				PageMasks[i] = BasePageMask << (i << PageMaskShift);
				PageMasksNegative[i] = ~PageMasks[i];
			}
		}
	}
}
