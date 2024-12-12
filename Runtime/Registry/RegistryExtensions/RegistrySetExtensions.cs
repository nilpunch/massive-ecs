using System;
using System.Runtime.CompilerServices;
using Unity.IL2CPP.CompilerServices;

namespace Massive
{
	[Il2CppSetOption(Option.NullChecks, false)]
	[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
	[Il2CppSetOption(Option.DivideByZeroChecks, false)]
	public static class RegistrySetExtensions
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static DataSet<T> DataSet<T>(this Registry registry)
		{
			if (registry.SetRegistry.Get<T>() is not DataSet<T> dataSet)
			{
				throw new Exception($"Type has no associated data! Maybe use {nameof(Set)}<T>() instead.");
			}

			return dataSet;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static SparseSet Set<T>(this Registry registry)
		{
			return registry.SetRegistry.Get<T>();
		}
	}
}
