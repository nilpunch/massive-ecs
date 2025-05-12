using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using Unity.IL2CPP.CompilerServices;

namespace Massive
{
	[Il2CppEagerStaticClassConstruction]
	[Il2CppSetOption(Option.NullChecks, false)]
	[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
	public static class AllocatorTypeId
	{
		private static readonly Dictionary<Type, AllocatorTypeIdInfo> s_typeInfo = new Dictionary<Type, AllocatorTypeIdInfo>();
		private static Type[] s_types = Array.Empty<Type>();
		private static int s_typeCounter = -1;

		public static AllocatorTypeIdInfo GetInfo(Type type)
		{
			if (!s_typeInfo.TryGetValue(type, out var typeIdInfo))
			{
				WarmupTypeId(type);
				typeIdInfo = s_typeInfo[type];
			}

			return typeIdInfo;
		}

		public static bool TryGetInfo(Type type, out AllocatorTypeIdInfo info)
		{
			return s_typeInfo.TryGetValue(type, out info);
		}

		public static Type GetTypeByIndex(int index)
		{
			if (index >= s_types.Length)
			{
				return null;
			}

			return s_types[index];
		}

		public static void WarmupTypeId(Type type)
		{
			try
			{
				var typeId = typeof(AllocatorTypeId<>).MakeGenericType(type);
				RuntimeHelpers.RunClassConstructor(typeId.TypeHandle);
			}
			catch
			{
				throw new Exception(
					$"The type identifier for {type} has been stripped from the build.\n" +
					"Ensure that the type is used in your codebase as a generic argument with the library API.");
			}
		}

		internal static ushort IncrementTypeCounter()
		{
			checked
			{
				return (ushort)Interlocked.Increment(ref s_typeCounter);
			}
		}

		internal static void Register(Type type, AllocatorTypeIdInfo info)
		{
			s_typeInfo.Add(type, info);

			if (info.Index >= s_types.Length)
			{
				Array.Resize(ref s_types, MathUtils.NextPowerOf2(info.Index + 1));
			}
			s_types[info.Index] = type;
		}
	}

	[Il2CppEagerStaticClassConstruction]
	[Il2CppSetOption(Option.NullChecks, false)]
	[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
	public static class AllocatorTypeId<T> where T : unmanaged
	{
		public static readonly AllocatorTypeIdInfo Info;

		static AllocatorTypeId()
		{
			var type = typeof(T);
			var index = AllocatorTypeId.IncrementTypeCounter();
			var typeName = type.GetFullGenericName();

			var info = new AllocatorTypeIdInfo(index, typeName);

			Info = info;

			AllocatorTypeId.Register(type, info);
		}
	}

	[Il2CppSetOption(Option.NullChecks, false)]
	[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
	public readonly struct AllocatorTypeIdInfo
	{
		public readonly ushort Index;
		public readonly string FullName;

		public AllocatorTypeIdInfo(ushort index, string fullName)
		{
			Index = index;
			FullName = fullName;
		}
	}
}
