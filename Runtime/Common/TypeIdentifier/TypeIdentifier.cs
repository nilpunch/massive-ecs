using System;
using System.Collections.Generic;
using System.Reflection;
using Unity.IL2CPP.CompilerServices;

// ReSharper disable all StaticMemberInGenericType
namespace Massive
{
	[Il2CppEagerStaticClassConstruction]
	[Il2CppSetOption(Option.NullChecks, false)]
	[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
	[Il2CppSetOption(Option.DivideByZeroChecks, false)]
	public abstract class TypeIdentifier<TSubset, T>
	{
		private static IdentifierInfo s_info;
		private static bool s_initialized;

		public static IdentifierInfo Info => s_info;

		static TypeIdentifier()
		{
			Warmup();
		}

		public static void Warmup()
		{
			if (s_initialized)
			{
				return;
			}

			s_info = TypeIdentifier<TSubset>.RegisterNew<T>();
			s_initialized = true;
		}
	}

	[Il2CppEagerStaticClassConstruction]
	[Il2CppSetOption(Option.NullChecks, false)]
	[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
	[Il2CppSetOption(Option.DivideByZeroChecks, false)]
	public abstract class TypeIdentifier<TSubset>
	{
		private static readonly Dictionary<Type, IdentifierInfo> s_runtimeTypeInfo = new Dictionary<Type, IdentifierInfo>();

		public static IdentifierInfo GetInfo(Type type)
		{
			if (!s_runtimeTypeInfo.TryGetValue(type, out var typeInfo))
			{
				var typeIdenifier = typeof(TypeIdentifier<>).MakeGenericType(typeof(TSubset), type);
				var warmup = typeIdenifier.GetMethod("Warmup", BindingFlags.Static | BindingFlags.Public);
				warmup.Invoke(null, null);

				typeInfo = s_runtimeTypeInfo[type];
			}

			return typeInfo;
		}

		internal static IdentifierInfo RegisterNew<T>()
		{
			var type = typeof(T);
			var index = TypesCounter<TSubset>.Increment();
			var typeName = type.GetFullGenericName();

			var info = new IdentifierInfo(index, typeName);
			s_runtimeTypeInfo.Add(type, info);

			return info;
		}
	}
}
