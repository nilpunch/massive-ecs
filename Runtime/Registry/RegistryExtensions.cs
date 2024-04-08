using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Massive
{
	public static class RegistryExtensions
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Entity GetEntity(this IRegistry registry, int id)
		{
			return registry.Entities.GetEntity(id);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static IGroup Group(this IRegistry registry, IReadOnlyList<ISet> owned = null,
			IReadOnlyList<IReadOnlySet> include = null, IReadOnlyList<IReadOnlySet> exclude = null)
		{
			return registry.Groups.EnsureGroup(owned, include, exclude);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ISet[] Many<T1>(this IRegistry registry) where T1 : struct
		{
			return new[] { registry.Any<T1>() };
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ISet[] Many<T1, T2>(this IRegistry registry) where T1 : struct where T2 : struct
		{
			return new[] { registry.Any<T1>(), registry.Any<T2>() };
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ISet[] Many<T1, T2, T3>(this IRegistry registry) where T1 : struct where T2 : struct where T3 : struct
		{
			return new[] { registry.Any<T1>(), registry.Any<T2>(), registry.Any<T3>() };
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Entity Create<T>(this IRegistry registry, T data = default) where T : struct
		{
			var id = registry.Create();
			registry.Add(id, data);
			return id;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void Destroy(this IRegistry registry, Entity entity)
		{
			if (!registry.Entities.IsAlive(entity))
			{
				return;
			}

			registry.Destroy(entity.Id);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool IsAlive(this IRegistry registry, Entity entity)
		{
			return registry.Entities.IsAlive(entity);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void Add<T>(this IRegistry registry, Entity entity, T data = default) where T : struct
		{
			if (!registry.Entities.IsAlive(entity))
			{
				return;
			}

			registry.Add(entity.Id, data);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void Remove<T>(this IRegistry registry, Entity entity) where T : struct
		{
			if (!registry.Entities.IsAlive(entity))
			{
				return;
			}

			registry.Remove<T>(entity.Id);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool Has<T>(this IRegistry registry, Entity entity) where T : struct
		{
			if (!registry.Entities.IsAlive(entity))
			{
				return false;
			}

			return registry.Has<T>(entity.Id);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ref T Get<T>(this IRegistry registry, Entity entity) where T : struct
		{
			if (!registry.Entities.IsAlive(entity))
			{
				throw new Exception("Entity is not alive!");
			}

			return ref registry.Get<T>(entity.Id);
		}
	}
}