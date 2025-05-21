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
	public static class AllocatorId
	{
		private static readonly Dictionary<Type, AllocatorIdInfo> s_typeInfo = new Dictionary<Type, AllocatorIdInfo>();
		private static Type[] s_types = Array.Empty<Type>();
		private static int s_typeCounter = -1;

		public static bool TryGetInfo(Type type, out AllocatorIdInfo info)
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

		internal static int IncrementTypeCounter()
		{
			return Interlocked.Increment(ref s_typeCounter);
		}

		internal static void Register(Type type, AllocatorIdInfo info)
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
	public static class AllocatorId<T> where T : unmanaged
	{
		public static readonly AllocatorIdInfo Info;

		/// <summary>
		/// Non-deterministic, don't store in simulation state. Used for lookups.
		/// </summary>
		public static readonly int Index;

		public static readonly string FullName;

		static AllocatorId()
		{
			var type = typeof(T);
			var index = AllocatorId.IncrementTypeCounter();
			var typeName = type.GetFullGenericName();

			var info = new AllocatorIdInfo(index, typeName);

			Info = info;
			Index = index;
			FullName = typeName;

			AllocatorId.Register(type, info);
		}
	}

	[Il2CppSetOption(Option.NullChecks, false)]
	[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
	public readonly struct AllocatorIdInfo
	{
		/// <summary>
		/// Non-deterministic, don't store in simulation state. Used for lookups.
		/// </summary>
		public readonly int Index;

		public readonly string FullName;

		public AllocatorIdInfo(int index, string fullName)
		{
			Index = index;
			FullName = fullName;
		}
	}
}
