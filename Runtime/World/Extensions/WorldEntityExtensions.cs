#if !MASSIVE_DISABLE_ASSERT
#define MASSIVE_ASSERT
#endif

using System.Runtime.CompilerServices;
using Unity.IL2CPP.CompilerServices;

namespace Massive
{
	[Il2CppSetOption(Option.NullChecks, false)]
	[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
	[Il2CppSetOption(Option.DivideByZeroChecks, false)]
	public static class WorldEntityExtensions
	{
		/// <summary>
		/// Returns alive entity for this ID.
		/// </summary>
		/// <remarks>
		/// Will throw an exception if the entity with this ID is not alive.
		/// </remarks>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Entity GetEntity(this World world, int id)
		{
			return world.Entities.GetEntity(id);
		}

		/// <summary>
		/// Creates a unique entity and returns it.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Entity CreateEntity(this World world)
		{
			return world.Entities.Create();
		}

		/// <summary>
		/// Creates a unique entity with the assigned component and returns it.
		/// Does not initialize component data.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Entity CreateEntity<T>(this World world)
		{
			var entity = world.CreateEntity();
			world.Assign<T>(entity.Id);
			return entity;
		}

		/// <summary>
		/// Creates a unique entity with the assigned component and returns it.
		/// </summary>
		/// <param name="data"> Initial data for the assigned component. </param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Entity CreateEntity<T>(this World world, T data)
		{
			var entity = world.CreateEntity();
			world.Assign(entity.Id, data);
			return entity;
		}

		/// <summary>
		/// Creates a unique entity with components of another entity.
		/// </summary>
		/// <remarks>
		/// Will throw an exception if the entity is not alive.
		/// </remarks>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Entity Clone(this World world, Entity entity)
		{
			Assert.IsAlive(world, entity);

			var cloneId = world.Clone(entity.Id);
			return world.GetEntity(cloneId);
		}

		/// <summary>
		/// Destroys this entity.
		/// </summary>
		/// <remarks>
		/// Will throw an exception if the entity is not alive.
		/// </remarks>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void Destroy(this World world, Entity entity)
		{
			Assert.IsAlive(world, entity);

			world.Entities.Destroy(entity.Id);
		}

		/// <summary>
		/// Checks whether the entity is alive.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool IsAlive(this World world, Entity entity)
		{
			return world.Entities.IsAlive(entity);
		}

		/// <summary>
		/// Assigns a component to the entity.
		/// </summary>
		/// <param name="data"> Initial data for the assigned component. </param>
		/// <remarks>
		/// Will throw an exception if the entity is not alive.
		/// </remarks>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void Assign<T>(this World world, Entity entity, T data)
		{
			Assert.IsAlive(world, entity);

			world.Assign(entity.Id, data);
		}

		/// <summary>
		/// Assigns a component to the entity, without data initialization.
		/// Repeat assignments are allowed.
		/// </summary>
		/// <remarks>
		/// Will throw an exception if the entity is not alive.
		/// </remarks>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void Assign<T>(this World world, Entity entity)
		{
			Assert.IsAlive(world, entity);

			world.Set<T>().Assign(entity.Id);
		}

		/// <summary>
		/// Unassigns a component from the entity.
		/// </summary>
		/// <remarks>
		/// Will throw an exception if the entity is not alive.
		/// </remarks>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void Unassign<T>(this World world, Entity entity)
		{
			Assert.IsAlive(world, entity);

			world.Set<T>().Unassign(entity.Id);
		}

		/// <summary>
		/// Checks whether the entity has such a component.
		/// </summary>
		/// <remarks>
		/// Will throw an exception if the entity is not alive.
		/// </remarks>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool Has<T>(this World world, Entity entity)
		{
			Assert.IsAlive(world, entity);

			return world.Set<T>().IsAssigned(entity.Id);
		}

		/// <summary>
		/// Returns a reference to the component of the entity.
		/// </summary>
		/// <remarks>
		/// Requesting a component from the entity that is being destroyed will throw an exception,
		/// and this method will throw an exception if the type has no associated data.
		/// </remarks>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ref T Get<T>(this World world, Entity entity)
		{
			Assert.IsAlive(world, entity);
			Assert.TypeHasData<T>(world, SuggestionMessage.DontUseGetWithEmptyTypes);

			return ref world.Get<T>(entity.Id);
		}
	}
}
