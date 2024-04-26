using System;
using System.Runtime.CompilerServices;

namespace Massive
{
	public static class RegistryIdExtensions
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int Create(this IRegistry registry)
		{
			return registry.Entities.Create().Id;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int Create<T>(this IRegistry registry, T data = default)
		{
			var id = registry.Create();
			registry.Assign(id, data);
			return id;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void Destroy(this IRegistry registry, int id)
		{
			registry.Entities.Destroy(id);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool IsAlive(this IRegistry registry, int id)
		{
			return registry.Entities.IsAlive(id);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void Assign<T>(this IRegistry registry, int id, T data = default)
		{
			var set = registry.Any<T>();
			if (set is IDataSet<T> dataSet)
			{
				dataSet.Assign(id, data);
			}
			else
			{
				set.Assign(id);
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void Unassign<T>(this IRegistry registry, int id)
		{
			registry.Any<T>().Unassign(id);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool Has<T>(this IRegistry registry, int id)
		{
			return registry.Any<T>().IsAssigned(id);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ref T Get<T>(this IRegistry registry, int id)
		{
			if (registry.Any<T>() is not IDataSet<T> dataSet)
			{
				throw new Exception("Type has no associated data!");
			}

			return ref dataSet.Get(id);
		}
	}
}
