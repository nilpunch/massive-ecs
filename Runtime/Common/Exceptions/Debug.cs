using System;
using System.Diagnostics;

namespace Massive
{
	public static class Debug
	{
		public const string Symbol = "MASSIVE_DEBUG";

		[Conditional(Symbol)]
		public static void Assert(bool condition, string message)
		{
			if (!condition)
			{
				throw new Exception(message);
			}
		}

		[Conditional(Symbol)]
		public static void Assert(Func<bool> condition, string message)
		{
			if (!condition())
			{
				throw new Exception(message);
			}
		}

		[Conditional(Symbol)]
		public static void AssertNotEmptyType<T>(Registry registry, string message)
		{
			if (registry.Set<T>() is not DataSet<T>)
			{
				throw new Exception(message);
			}
		}
	}
}
