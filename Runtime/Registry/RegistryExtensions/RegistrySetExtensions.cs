using System;
using System.Runtime.CompilerServices;

namespace Massive
{
	public static class RegistrySetExtensions
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static IDataSet<T> Components<T>(this IRegistry registry)
		{
			if (registry.Any<T>() is not IDataSet<T> dataSet)
			{
				throw new Exception($"Type has no associated data! Maybe use {nameof(Any)}<T>() instead.");
			}

			return dataSet;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ISet Any<T>(this IRegistry registry)
		{
			return registry.SetRegistry.Get<T>();
		}
	}
}
