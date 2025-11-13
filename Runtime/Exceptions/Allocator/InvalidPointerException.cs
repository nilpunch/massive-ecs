using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Massive
{
	public class InvalidPointerException : MassiveException
	{
		private InvalidPointerException(string message) : base(message)
		{
		}

		[Conditional(Condition)]
		[MethodImpl(MethodImplOptions.NoInlining)]
		public static void ThrowIfNotAllocated(Allocator allocator, Pointer pointer)
		{
			if (!allocator.IsAllocated(pointer))
			{
				throw new InvalidPointerException($"Pointer with page:{pointer.Page} offset:{pointer.Offset} not allocated.");
			}
		}
	}
}
