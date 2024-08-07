using System;
using System.Runtime.CompilerServices;

namespace Massive
{
	public static class RegistrySetExtensions
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static DataSet<T> DataSet<T>(this Registry registry)
		{
			if (registry.Set<T>() is not DataSet<T> dataSet)
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
