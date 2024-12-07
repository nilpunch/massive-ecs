using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;
using Unity.IL2CPP.CompilerServices;

// ReSharper disable all StaticMemberInGenericType
namespace Massive
{
	[Il2CppEagerStaticClassConstruction]
	[Il2CppSetOption(Option.NullChecks, false)]
	[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
	[Il2CppSetOption(Option.DivideByZeroChecks, false)]
	public static class TypeIdentifier<T>
	{
		public static TypeIdentifier.TypeInfo Info;
		private static bool s_initialized;

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

			Info = TypeIdentifier.RegisterNew<T>();
			s_initialized = true;
		}
	}

	[Il2CppEagerStaticClassConstruction]
	[Il2CppSetOption(Option.NullChecks, false)]
	[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
	[Il2CppSetOption(Option.DivideByZeroChecks, false)]
	public static class TypeIdentifier
	{
		private static readonly Dictionary<Type, TypeInfo> s_runtimeTypeInfo = new Dictionary<Type, TypeInfo>();

		public static TypeInfo GetInfo(Type type)
		{
			if (!s_runtimeTypeInfo.TryGetValue(type, out var typeInfo))
			{
				Warmup(type);
				typeInfo = s_runtimeTypeInfo[type];
			}
			return typeInfo;
		}

		private static void Warmup(Type type)
		{
			var typeIdenifier = typeof(TypeIdentifier<>).MakeGenericType(type);
			var warmup = typeIdenifier.GetMethod("Warmup", BindingFlags.Static | BindingFlags.Public);
			warmup.Invoke(null, null);
		}

		internal static TypeInfo RegisterNew<T>()
		{
			var type = typeof(T);
			var index = TypesCounter.Increment();
			var typeName = type.GetFullGenericName();

			var info = new TypeInfo(index, typeName);
			s_runtimeTypeInfo.Add(type, info);

			return info;
		}

		public struct TypeInfo
		{
			public readonly int Index;
			public readonly string FullName;

			public TypeInfo(int index, string fullName)
			{
				Index = index;
				FullName = fullName;
			}
		}
	}

	[Il2CppEagerStaticClassConstruction]
	[Il2CppSetOption(Option.NullChecks, false)]
	[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
	[Il2CppSetOption(Option.DivideByZeroChecks, false)]
	internal static class TypesCounter
	{
		private static int s_value;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static int Increment()
		{
			return Interlocked.Increment(ref s_value);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static int Get()
		{
			return s_value;
		}
	}
}
