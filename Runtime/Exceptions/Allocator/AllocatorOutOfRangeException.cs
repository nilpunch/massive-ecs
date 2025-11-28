using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Massive
{
	public class AllocatorOutOfRangeException : MassiveException
	{
		private AllocatorOutOfRangeException(string message) : base(message)
		{
		}

		[Conditional(Condition)]
		[MethodImpl(MethodImplOptions.NoInlining)]
		public static void ThrowIfOutOfRangeInclusive(int index, int rangeInclusive)
		{
			if (index < 0 || index > rangeInclusive)
			{
				throw new AllocatorOutOfRangeException($"Index {index} was out of range. Must be non-negative and not greater than {rangeInclusive}.");
			}
		}

		[Conditional(Condition)]
		[MethodImpl(MethodImplOptions.NoInlining)]
		public static void ThrowIfOutOfRangeExclusive(int index, int rangeExclusive)
		{
			if (index < 0 || index >= rangeExclusive)
			{
				throw new AllocatorOutOfRangeException($"Index {index} was out of range. Must be non-negative and less than {rangeExclusive}.");
			}
		}

		[Conditional(Condition)]
		[MethodImpl(MethodImplOptions.NoInlining)]
		public static void ThrowIfNotFitsInSlot(int elementSize, int slotSize)
		{
			if (elementSize > slotSize)
			{
				throw new AllocatorOutOfRangeException($"Element size {elementSize} exceeds available slot size {slotSize}.");
			}
		}
	}
}
