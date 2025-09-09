using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using Unity.IL2CPP.CompilerServices;

// ReSharper disable all StaticMemberInGenericType
namespace Massive
{
	[Il2CppEagerStaticClassConstruction]
	[Il2CppSetOption(Option.NullChecks, false)]
	[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
	public static class ComponentId
	{
		private static readonly Dictionary<Type, TypeIdInfo> s_typeInfo = new Dictionary<Type, TypeIdInfo>();
		private static Type[] s_components = Array.Empty<Type>();
		private static int s_componentsCounter = -1;

		[MethodImpl(MethodImplOptions.NoInlining)]
		public static TypeIdInfo GetInfo(Type type)
		{
			if (!s_typeInfo.TryGetValue(type, out var typeIdInfo))
			{
				WarmupCopmonentId(type);
				typeIdInfo = s_typeInfo[type];
			}

			return typeIdInfo;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool TryGetInfo(Type type, out TypeIdInfo info)
		{
			return s_typeInfo.TryGetValue(type, out info);
		}

		public static Type GetTypeByIndex(int index)
		{
			if (index >= s_components.Length)
			{
				return null;
			}

			return s_components[index];
		}

		public static void WarmupCopmonentId(Type type)
		{
			try
			{
				var typeId = typeof(ComponentId<>).MakeGenericType(type);
				RuntimeHelpers.RunClassConstructor(typeId.TypeHandle);
			}
			catch
			{
				MassiveException.Throw(
					$"The type identifier for {type} has been stripped from the build.\n" +
					"Ensure that the type is used in your codebase as a generic argument with the library API.");
			}
		}

		internal static int IncrementTypeCounter()
		{
			return Interlocked.Increment(ref s_componentsCounter);
		}

		internal static void Register(Type type, TypeIdInfo info)
		{
			s_typeInfo.Add(type, info);

			if (info.Index >= s_components.Length)
			{
				Array.Resize(ref s_components, MathUtils.NextPowerOf2(info.Index + 1));
			}
			s_components[info.Index] = type;
		}
	}

	[Il2CppEagerStaticClassConstruction]
	[Il2CppSetOption(Option.NullChecks, false)]
	[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
	public static class ComponentId<T>
	{
		public static readonly TypeIdInfo Info;

		static ComponentId()
		{
			var type = typeof(T);
			var index = ComponentId.IncrementTypeCounter();
			var typeName = type.GetFullGenericName();

			var info = new TypeIdInfo(index, typeName, type);

			Info = info;

			ComponentId.Register(type, info);
		}
	}
}
