namespace Massive
{
	public static class Constants
	{
		public const int DefaultFramesCapacity = 121;

		public const int InvalidId = -1;

		/// <summary>
		/// Must be power of 2 in range [64, 4096].
		/// </summary>
		public const int PageSize = 256;
		public const int PageSizePower = 8;
		public const int PageSizeMinusOne = PageSize - 1;
		public const ulong PageMask = (1UL << (PageSize >> 6)) - 1;
		public const int PagesInBits1MinusOne = ((1 << 12) >> PageSizePower) - 1;
		public const int PageMaskShiftPower = PageSizePower - 6;
	}
}
