﻿using System;
using System.Runtime.CompilerServices;

namespace Massive
{
	public static class RegistryEntityExtensions
	{
		/// <summary>
		/// Returns alive entity for this ID.
		/// </summary>
		/// <remarks> If an entity with this ID is not alive, the behavior is undefined. </remarks>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Entity GetEntity(this IRegistry registry, int id)
		{
			return registry.Entities.GetEntity(id);
		}

		/// <summary>
		/// Creates a unique entity and returns it.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Entity CreateEntity(this IRegistry registry)
		{
			return registry.Entities.Create();
		}

		/// <summary>
		/// Creates a unique entity with the assigned component and returns it.
		/// </summary>
		/// <param name="data"> Initial data for the assigned component. </param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Entity CreateEntity<T>(this IRegistry registry, T data = default)
		{
			var entity = registry.CreateEntity();
			registry.Assign(entity.Id, data);
			return entity;
		}

		/// <summary>
		/// Destroys this entity if alive.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void Destroy(this IRegistry registry, Entity entity)
		{
			if (!registry.Entities.IsAlive(entity))
			{
				return;
			}

			registry.Destroy(entity.Id);
		}

		/// <summary>
		/// Checks whether an entity is alive.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool IsAlive(this IRegistry registry, Entity entity)
		{
			return registry.Entities.IsAlive(entity);
		}

		/// <summary>
		/// Assigns a component to an entity.
		/// </summary>
		/// <param name="data"> Initial data for the assigned component. </param>
		/// <remarks> If the entity is not alive, nothing happens. </remarks>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void Assign<T>(this IRegistry registry, Entity entity, T data = default)
		{
			if (!registry.Entities.IsAlive(entity))
			{
				return;
			}

			registry.Assign(entity.Id, data);
		}

		/// <summary>
		/// Unassigns a component from an entity.
		/// </summary>
		/// <remarks> If the entity is not alive, nothing happens. </remarks>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void Unassign<T>(this IRegistry registry, Entity entity)
		{
			if (!registry.Entities.IsAlive(entity))
			{
				return;
			}

			registry.Unassign<T>(entity.Id);
		}

		/// <summary>
		/// Checks whether an entity has such a component.
		/// </summary>
		/// <remarks> Returns false if the entity is not alive. </remarks>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool Has<T>(this IRegistry registry, Entity entity)
		{
			if (!registry.Entities.IsAlive(entity))
			{
				return false;
			}

			return registry.Has<T>(entity.Id);
		}

		/// <summary>
		/// Returns a reference to the component of an entity.
		/// </summary>
		/// <remarks>
		/// Requesting a component from an entity that is being destroyed will throw an exception,
		/// and this method may throw an exception if the type has no associated data.
		/// </remarks>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ref T Get<T>(this IRegistry registry, Entity entity)
		{
			if (!registry.Entities.IsAlive(entity))
			{
				throw new Exception("Entity is not alive!");
			}

			return ref registry.Get<T>(entity.Id);
		}
	}
}
