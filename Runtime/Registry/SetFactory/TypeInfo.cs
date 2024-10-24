using System;
using System.Reflection;

namespace Massive
{
	internal static class TypeInfo
	{
		public static bool HasNoFields(Type type)
		{
			return !HasAnyFields(type);
		}

		public static bool HasAnyFields(Type type)
		{
			return type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic).Length > 0;
		}
	}
}
