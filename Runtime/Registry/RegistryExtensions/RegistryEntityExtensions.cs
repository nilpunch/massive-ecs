﻿#if !MASSIVE_DISABLE_ASSERT
#define MASSIVE_ASSERT
#endif

using System.Runtime.CompilerServices;
using Unity.IL2CPP.CompilerServices;

namespace Massive
{
	[Il2CppSetOption(Option.NullChecks, false)]
	[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
	[Il2CppSetOption(Option.DivideByZeroChecks, false)]
	public static class RegistryEntityExtensions
	{
		/// <summary>
		/// Returns alive entity for this ID.
		/// </summary>
		/// <remarks>
		/// Will throw an exception if the entity with this ID is not alive.
		/// </remarks>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Entity GetEntity(this Registry registry, int id)
		{
			return registry.Entities.GetEntity(id);
		}

		/// <summary>
		/// Creates a unique entity and returns it.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Entity CreateEntity(this Registry registry)
		{
			return registry.Entities.Create();
		}

		/// <summary>
		/// Creates a unique entity with the assigned component and returns it.
		/// Does not initialize component data.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Entity CreateEntity<T>(this Registry registry)
		{
			var entity = registry.CreateEntity();
			registry.Assign<T>(entity.Id);
			return entity;
		}

		/// <summary>
		/// Creates a unique entity with the assigned component and returns it.
		/// </summary>
		/// <param name="data"> Initial data for the assigned component. </param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Entity CreateEntity<T>(this Registry registry, T data)
		{
			var entity = registry.CreateEntity();
			registry.Assign(entity.Id, data);
			return entity;
		}

		/// <summary>
		/// Creates a unique entity with components of another entity.
		/// </summary>
		/// <remarks>
		/// Will throw an exception if the entity is not alive.
		/// </remarks>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Entity Clone(this Registry registry, Entity entity)
		{
			Assert.IsAlive(registry, entity);

			var cloneId = registry.Clone(entity.Id);
			return registry.GetEntity(cloneId);
		}

		/// <summary>
		/// Destroys this entity.
		/// </summary>
		/// <remarks>
		/// Will throw an exception if the entity is not alive.
		/// </remarks>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void Destroy(this Registry registry, Entity entity)
		{
			Assert.IsAlive(registry, entity);

			registry.Entities.Destroy(entity.Id);
		}

		/// <summary>
		/// Checks whether the entity is alive.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool IsAlive(this Registry registry, Entity entity)
		{
			return registry.Entities.IsAlive(entity);
		}

		/// <summary>
		/// Assigns a component to the entity.
		/// </summary>
		/// <param name="data"> Initial data for the assigned component. </param>
		/// <remarks>
		/// Will throw an exception if the entity is not alive.
		/// </remarks>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void Assign<T>(this Registry registry, Entity entity, T data)
		{
			Assert.IsAlive(registry, entity);

			registry.Assign(entity.Id, data);
		}

		/// <summary>
		/// Assigns a component to the entity, without data initialization.
		/// Repeat assignments are allowed.
		/// </summary>
		/// <remarks>
		/// Will throw an exception if the entity is not alive.
		/// </remarks>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void Assign<T>(this Registry registry, Entity entity)
		{
			Assert.IsAlive(registry, entity);

			registry.Set<T>().Assign(entity.Id);
		}

		/// <summary>
		/// Unassigns a component from the entity.
		/// </summary>
		/// <remarks>
		/// Will throw an exception if the entity is not alive.
		/// </remarks>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void Unassign<T>(this Registry registry, Entity entity)
		{
			Assert.IsAlive(registry, entity);

			registry.Set<T>().Unassign(entity.Id);
		}

		/// <summary>
		/// Checks whether the entity has such a component.
		/// </summary>
		/// <remarks>
		/// Will throw an exception if the entity is not alive.
		/// </remarks>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool Has<T>(this Registry registry, Entity entity)
		{
			Assert.IsAlive(registry, entity);

			return registry.Set<T>().IsAssigned(entity.Id);
		}

		/// <summary>
		/// Returns a reference to the component of the entity.
		/// </summary>
		/// <remarks>
		/// Requesting a component from the entity that is being destroyed will throw an exception,
		/// and this method will throw an exception if the type has no associated data.
		/// </remarks>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ref T Get<T>(this Registry registry, Entity entity)
		{
			Assert.IsAlive(registry, entity);
			Assert.TypeHasData<T>(registry, SuggestionMessage.DontUseGetWithEmptyTypes);

			return ref registry.Get<T>(entity.Id);
		}
	}
}
