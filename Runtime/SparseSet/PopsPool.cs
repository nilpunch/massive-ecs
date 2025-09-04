using System;
using System.Runtime.CompilerServices;

namespace Massive
{
	public static class PopsPool
	{
		public static int Count { get; set; }

		public static Pops[] Pool { get; set; } = Array.Empty<Pops>();

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Pops Rent()
		{
			if (Count > 0)
			{
				return Pool[--Count];
			}

			return new Pops();
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void ReturnAndPop(Pops pop)
		{
			if (Count >= Pool.Length)
			{
				Pool = Pool.Resize(MathUtils.NextPowerOf2(Count + 1));
			}

			pop.PopAll();
			Pool[Count++] = pop;
		}
	}
}
