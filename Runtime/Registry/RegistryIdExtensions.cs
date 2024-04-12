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
		public static int Create<T>(this IRegistry registry, T data = default) where T : struct
		{
			var id = registry.Entities.Create().Id;
			registry.Add(id, data);
			return id;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void Destroy(this IRegistry registry, int id)
		{
			registry.Entities.Delete(id);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool IsAlive(this IRegistry registry, int id)
		{
			return registry.Entities.IsAlive(id);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void Add<T>(this IRegistry registry, int id, T data = default) where T : struct
		{
			var set = registry.Any<T>();
			if (set is IDataSet<T> dataSet)
			{
				dataSet.Ensure(id, data);
			}
			else
			{
				set.Ensure(id);
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void Remove<T>(this IRegistry registry, int id) where T : struct
		{
			registry.Any<T>().Remove(id);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool Has<T>(this IRegistry registry, int id) where T : struct
		{
			return registry.Any<T>().IsAlive(id);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ref T Get<T>(this IRegistry registry, int id) where T : struct
		{
			if (registry.Any<T>() is not IDataSet<T> dataSet)
			{
				throw new Exception("Type has no associated data!");
			}

			return ref dataSet.Get(id);
		}
	}
}