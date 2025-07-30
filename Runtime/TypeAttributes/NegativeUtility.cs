using System;

namespace Massive
{
	public static class NegativeUtility
	{
		public static Type Collapse(Type type)
		{
			var depth = 0;

			while (IsNotType(type))
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

		public static bool IsNotType(Type type)
		{
			return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Not<>);
		}
	}
}
