using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Massive
{
	public class MassiveException : Exception
	{
		public const string Condition = "MASSIVE_ASSERT";

		protected MassiveException(string message) : base($"[{Constants.LibraryName}] {message}")
		{
		}

		[Conditional(Condition)]
		[MethodImpl(MethodImplOptions.NoInlining)]
		public static void Throw(string message)
		{
			throw new MassiveException(message);
		}
	}
}
