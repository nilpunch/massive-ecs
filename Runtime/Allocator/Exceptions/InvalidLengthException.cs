using System;
using System.Runtime.CompilerServices;

namespace Massive
{
	public class InvalidLengthException : Exception
	{
		private InvalidLengthException(string message) : base(message)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public static void Throw(int length)
		{
			if (length < 0)
			{
				throw new InvalidLengthException($"{MassiveAssert.Library} Provided length {length} is negative.");
			}
		}
	}
}
