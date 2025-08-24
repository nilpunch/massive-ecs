using System;
using System.Runtime.CompilerServices;

namespace Massive
{
	public static class NegativeUtils
	{
		[MethodImpl(MethodImplOptions.NoInlining)]
		public static Type CollapseNegations(Type type)
		{
			var depth = 0;

			while (IsNegative(type))
			{
				type = type.GetGenericArguments()[0];
				depth++;
			}

			// Even depth cancels out.
			if (depth % 2 == 0)
			{
				return type;
			}

			// Odd depth keeps one Not<>.
			return typeof(Not<>).MakeGenericType(type);
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public static bool IsNegative(Type type)
		{
			return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Not<>);
		}
	}
}
