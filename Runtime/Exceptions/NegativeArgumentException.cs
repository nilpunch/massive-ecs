using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Massive
{
	public class NegativeArgumentException : MassiveException
	{
		private NegativeArgumentException(string message) : base(message)
		{
		}

		[Conditional(Condition)]
		[MethodImpl(MethodImplOptions.NoInlining)]
		public static void ThrowIfNegative(int value)
		{
			if (value < 0)
			{
				throw new NegativeArgumentException($"Provided argument {value} is negative.");
			}
		}
	}
}
