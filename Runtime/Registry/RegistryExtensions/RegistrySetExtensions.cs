using System;
using System.Runtime.CompilerServices;

namespace Massive
{
	public static class RegistrySetExtensions
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static IDataSet<T> DataSet<T>(this IRegistry registry)
		{
			if (registry.Set<T>() is not IDataSet<T> dataSet)
			{
				throw new Exception($"Type has no associated data! Maybe use {nameof(Set)}<T>() instead.");
			}

			return dataSet;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ISet Set<T>(this IRegistry registry)
		{
			return registry.SetRegistry.Get<T>();
		}
	}
}
