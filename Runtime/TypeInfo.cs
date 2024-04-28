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

#if UNITY_2020_3_OR_NEWER
		[UnityEngine.Scripting.Preserve]
#endif
		private static void VirtualGenericsCompilerHint()
		{
			// ReSharper disable ReturnValueOfPureMethodIsNotUsed
			new NormalSetFactory().CreateAppropriateSet<T>();
			new MassiveSetFactory().CreateAppropriateSet<T>();
		}
	}
}
