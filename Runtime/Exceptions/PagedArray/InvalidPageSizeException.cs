using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Massive
{
	public class InvalidPageSizeException : MassiveException
	{
		private InvalidPageSizeException(string message) : base(message)
		{
		}

		[Conditional(Condition)]
		[MethodImpl(MethodImplOptions.NoInlining)]
		public static void ThrowIfNotPowerOf2<T>(int pageSize)
		{
			if (!MathUtils.IsPowerOfTwo(pageSize))
			{
				throw new InvalidPageSizeException($"Page size must be power of two! Type:{typeof(T).GetGenericName()}.");
			}
		}

		[Conditional(Condition)]
		[MethodImpl(MethodImplOptions.NoInlining)]
		public static void ThrowIfTooLargeOrTooSmall<T>(int pageSize)
		{
			if (pageSize < 64)
			{
				throw new InvalidPageSizeException($"Page size must be at least 64! Type:{typeof(T).GetGenericName()}.");
			}

			if (pageSize > 4096)
			{
				throw new InvalidPageSizeException($"Page size must be at most 4096! Type:{typeof(T).GetGenericName()}.");
			}
		}
	}
}
