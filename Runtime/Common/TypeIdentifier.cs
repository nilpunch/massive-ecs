using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using Unity.IL2CPP.CompilerServices;

namespace Massive
{
	public class TypeInfo
	{
		public readonly int Index;
		public readonly string FullName;

		public TypeInfo(int index, string fullName)
		{
			Index = index;
			FullName = fullName;
		}
	}

	[Il2CppEagerStaticClassConstruction]
	[Il2CppSetOption(Option.NullChecks, false)]
	[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
	[Il2CppSetOption(Option.DivideByZeroChecks, false)]
	public static class CommonTypeIdentifier
	{
		internal static readonly Dictionary<Type, TypeInfo> RuntimeTypeInfo = new Dictionary<Type, TypeInfo>();
		internal static int NextFreeIndex;

		public static TypeInfo Get(Type type)
		{
			if (!RuntimeTypeInfo.TryGetValue(type, out var typeInfo))
			{
				Warmup(type);
				typeInfo = RuntimeTypeInfo[type];
			}
			return typeInfo;
		}

		private static void Warmup(Type type)
		{
			var typeId = typeof(TypeIdentifier<>).MakeGenericType(type);
			var warm = typeId.GetMethod("Warmup", BindingFlags.Static | BindingFlags.Public);
			warm.Invoke(null, null);
		}
	}

	[Il2CppEagerStaticClassConstruction]
	[Il2CppSetOption(Option.NullChecks, false)]
	[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
	[Il2CppSetOption(Option.DivideByZeroChecks, false)]
	public static class TypeIdentifier<T>
	{
		// ReSharper disable once StaticMemberInGenericType
		public static TypeInfo Info;

		static TypeIdentifier()
		{
			Warmup();
		}

		public static void Warmup()
		{
			if (Info != null)
			{
				return;
			}

			var type = typeof(T);

			var index = Interlocked.Increment(ref CommonTypeIdentifier.NextFreeIndex);
			var typeName = typeof(T).GetFullGenericName();

			Info = new TypeInfo(index, typeName);
			CommonTypeIdentifier.RuntimeTypeInfo.Add(type, Info);
		}
	}
}
