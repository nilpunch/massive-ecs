using System.Reflection;

namespace Massive
{
	internal static class TypeInfo<T>
	{
		// ReSharper disable StaticMemberInGenericType
		public static bool HasAnyFields { get; }
		public static bool HasNoFields { get; }

		static TypeInfo()
		{
			HasAnyFields = typeof(T).GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic).Length > 0;
			HasNoFields = !HasAnyFields;
		}
	}
}
