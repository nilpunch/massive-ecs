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
	}
}
