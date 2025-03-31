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
	public abstract class RuntimeTypeId
	{
		private static readonly Dictionary<Type, TypeIdInfo> s_typeInfo = new Dictionary<Type, TypeIdInfo>();
		private static Type[] s_types = Array.Empty<Type>();

		public static TypeIdInfo GetInfo(Type type)
		{
			if (!s_typeInfo.TryGetValue(type, out var identifierInfo))
			{
				WarmupIdentifier(type);
				identifierInfo = s_typeInfo[type];
			}

			return identifierInfo;
		}

		public static Type GetTypeByIndex(int index)
		{
			if (index >= s_types.Length)
			{
				return null;
			}

			return s_types[index];
		}

		internal static void Register(Type type, TypeIdInfo info)
		{
			s_typeInfo.Add(type, info);

			if (info.Index >= s_types.Length)
			{
				s_types = s_types.Resize(MathUtils.NextPowerOf2(info.Index + 1));
			}
			s_types[info.Index] = type;
		}

		private static void WarmupIdentifier(Type type)
		{
			try
			{
				var typeIdenifier = typeof(TypeId<>).MakeGenericType(type);
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
