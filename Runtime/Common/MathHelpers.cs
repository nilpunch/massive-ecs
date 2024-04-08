using System.Runtime.CompilerServices;

namespace Massive
{
	public static class MathHelpers
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int GetNextPowerOf2(int value)
		{
			uint v = (uint)value;

			v--;
			v |= v >> 1;
			v |= v >> 2;
			v |= v >> 4;
			v |= v >> 8;
			v |= v >> 16;
			v++;

			return (int)v;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int CombineHashes(int a, int b)
		{
			unchecked
			{
				int hash = 17;
				hash = hash * 31 + a;
				hash = hash * 31 + b;
				return hash;
			}
		}
	}
}