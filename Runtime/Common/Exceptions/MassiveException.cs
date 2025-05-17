using System;

namespace Massive
{
	public abstract class MassiveException : Exception
	{
		protected MassiveException(string message) : base($"[MASSIVE] {message}")
		{
		}

		public const string Condition = "MASSIVE_ASSERT";
	}
}
