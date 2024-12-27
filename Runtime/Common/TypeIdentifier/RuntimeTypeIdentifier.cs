using System;
using System.Collections.Generic;
using System.Reflection;
using Unity.IL2CPP.CompilerServices;

namespace Massive
{
	[Il2CppEagerStaticClassConstruction]
	[Il2CppSetOption(Option.NullChecks, false)]
	[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
	[Il2CppSetOption(Option.DivideByZeroChecks, false)]
	public abstract class RuntimeTypeIdentifier
	{
		private static readonly Dictionary<Type, IdentifierInfo> s_runtimeIdentifierInfo = new Dictionary<Type, IdentifierInfo>();

		public static IdentifierInfo GetInfo(Type type)
		{
			if (!s_runtimeIdentifierInfo.TryGetValue(type, out var identifierInfo))
			{
				WarmupIdentifier(type);
				identifierInfo = s_runtimeIdentifierInfo[type];
			}

			return identifierInfo;
		}

		internal static void Register(Type type, IdentifierInfo info)
		{
			s_runtimeIdentifierInfo.Add(type, info);
		}

		private static void WarmupIdentifier(Type type)
		{
			try
			{
				var typeIdenifier = typeof(TypeIdentifier<>).MakeGenericType(type);
				var warmup = typeIdenifier.GetMethod("Warmup", BindingFlags.Static | BindingFlags.Public);
				warmup.Invoke(null, null);
			}
			catch
			{
				throw new Exception(
					$"The type identifier for {type} has been stripped from the build.\n" +
					"Ensure that the type is used in your codebase as a generic argument with the library API.");
			}
		}
	}
}
