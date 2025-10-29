using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Massive
{
	public class NotPowerOfTwoArgumentException : MassiveException
	{
		private NotPowerOfTwoArgumentException(string message) : base(message)
		{
		}

		[Conditional(Condition)]
		[MethodImpl(MethodImplOptions.NoInlining)]
		public static void ThrowIfNotPowerOfTwo(int value)
		{
			if (!MathUtils.IsPowerOfTwo(value))
			{
				throw new NotPowerOfTwoArgumentException($"Provided argument {value} is not power of 2.");
			}
		}
	}
}
