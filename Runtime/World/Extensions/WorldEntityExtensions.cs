using System.Runtime.CompilerServices;
using Unity.IL2CPP.CompilerServices;

namespace Massive
{
	[Il2CppEagerStaticClassConstruction]
	[Il2CppSetOption(Option.NullChecks, false)]
	[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
	public static class WorldEntityExtensions
	{
		/// <summary>
		/// Returns entity for this ID.
		/// </summary>
		/// <remarks>
		/// Throws if the entity with this ID is not alive.
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
			return new Entity(world.Entities.Create(), world);
		}

		/// <summary>
		/// Creates a unique entity, adds a component without initializing data, and returns the entity.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Entity CreateEntity<T>(this World world)
		{
			var entity = world.CreateEntity();
			world.Add<T>(entity.Id);
			return entity;
		}

		/// <summary>
		/// Creates a unique entity, adds a component with provided data, and returns the entity.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Entity CreateEntity<T>(this World world, T data)
		{
			var entity = world.CreateEntity();
			world.Set(entity.Id, data);
			return entity;
		}
	}
}
