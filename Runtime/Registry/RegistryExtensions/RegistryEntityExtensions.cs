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
		/// If an entity with this ID is not alive, will throw an exception.
		/// </remarks>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Entity GetEntity(this Registry registry, int id)
		{
			Debug.AssertEntityAlive(registry, id);

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
		/// Cloning entity that is not alive will throw an exception.
		/// </remarks>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Entity Clone(this Registry registry, Entity entity)
		{
			Debug.AssertEntityAlive(registry, entity);

			var cloneId = registry.Clone(entity.Id);
			return registry.GetEntity(cloneId);
		}

		/// <summary>
		/// Destroys this entity if alive.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void Destroy(this Registry registry, Entity entity)
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
		public static bool IsAlive(this Registry registry, Entity entity)
		{
			return registry.Entities.IsAlive(entity);
		}

		/// <summary>
		/// Assigns a component to an entity.
		/// </summary>
		/// <param name="data"> Initial data for the assigned component. </param>
		/// <remarks>
		/// Assigning a component to an entity that is being destroyed will throw an exception.
		/// </remarks>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void Assign<T>(this Registry registry, Entity entity, T data)
		{
			Debug.AssertEntityAlive(registry, entity);

			registry.Assign(entity.Id, data);
		}

		/// <summary>
		/// Assigns a component to an entity, without data initialization.
		/// Repeat assignments are allowed.
		/// </summary>
		/// <remarks>
		/// Assigning a component to an entity that is being destroyed will throw an exception.
		/// </remarks>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void Assign<T>(this Registry registry, Entity entity)
		{
			Debug.AssertEntityAlive(registry, entity);

			registry.Assign<T>(entity.Id);
		}

		/// <summary>
		/// Unassigns a component from an entity.
		/// </summary>
		/// <remarks>
		/// If the entity is not alive or does not have the component, nothing happens.
		/// </remarks>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void Unassign<T>(this Registry registry, Entity entity)
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
		/// <remarks>
		/// Returns false if the entity is not alive.
		/// </remarks>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool Has<T>(this Registry registry, Entity entity)
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
		/// and this method will throw an exception if the type has no associated data.
		/// </remarks>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ref T Get<T>(this Registry registry, Entity entity)
		{
			Debug.AssertEntityAlive(registry, entity);
			Debug.AssertNotEmptyType<T>(registry, SuggestionMessage.DontUseGet);

			return ref registry.Get<T>(entity.Id);
		}
	}
}
