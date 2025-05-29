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
	public static class AllocatorId<T> where T : unmanaged
	{
		/// <summary>
		/// Non-deterministic, used for lookups.<br/>
		/// Don't store it in simulation.
		/// </summary>
		public static readonly int Index;

		public static readonly string FullName;

		static AllocatorId()
		{
			var type = typeof(T);
			var index = AllocatorId.IncrementTypeCounter();
			var typeName = type.GetFullGenericName();

			Index = index;
			FullName = typeName;

			AllocatorId.Register(type, new AllocatorIdInfo(index, typeName));
		}
	}

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

	[Il2CppSetOption(Option.NullChecks, false)]
	[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
	public readonly struct AllocatorIdInfo
	{
		/// <summary>
		/// Non-deterministic, used for lookups.<br/>
		/// Don't store it in simulation.
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
