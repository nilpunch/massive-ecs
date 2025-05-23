using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Massive
{
	public class AllocatorIndexOutOfRangeException : MassiveException
	{
		private AllocatorIndexOutOfRangeException(string message) : base(message)
		{
		}

		[Conditional(Condition)]
		[MethodImpl(MethodImplOptions.NoInlining)]
		public static void ThrowIfOutOfRangeInclusive(int index, int rangeInclusive)
		{
			if (index < 0 || index > rangeInclusive)
			{
				throw new AllocatorIndexOutOfRangeException($"Index {index} was out of range. Must be non-negative and not greater than {rangeInclusive}.");
			}
		}

		[Conditional(Condition)]
		[MethodImpl(MethodImplOptions.NoInlining)]
		public static void ThrowIfOutOfRangeExclusive(int index, int rangeExclusive)
		{
			if (index < 0 || index >= rangeExclusive)
			{
				throw new AllocatorIndexOutOfRangeException($"Index {index} was out of range. Must be non-negative and less than {rangeExclusive}.");
			}
		}
	}
}
