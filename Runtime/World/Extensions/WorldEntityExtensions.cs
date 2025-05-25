#if !MASSIVE_DISABLE_ASSERT
#define MASSIVE_ASSERT
#endif

using System.Runtime.CompilerServices;
using Unity.IL2CPP.CompilerServices;

namespace Massive
{
	[Il2CppSetOption(Option.NullChecks, false)]
	[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
	public static class WorldEntityExtensions
	{
		/// <summary>
		/// Returns alive entity for this ID.
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
			return world.Entities.Create();
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

		/// <summary>
		/// Creates a unique entity with components of another entity.<br/>
		/// Makes shallow copy of each component.
		/// </summary>
		/// <remarks>
		/// Throws if the entity is not alive.
		/// </remarks>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Entity Clone(this World world, Entity entity)
		{
			InvalidCloneOperationException.ThrowIfEntityDead(world.Entities, entity);

			var entityId = entity.Id;
			var clone = world.CreateEntity();
			var cloneId = clone.Id;

			var setList = world.Sets.AllSets;
			var setCount = setList.Count;
			var sets = setList.Items;
			for (var i = 0; i < setCount; i++)
			{
				var set = sets[i];
				var index = set.GetIndexOrNegative(entityId);
				if (index >= 0)
				{
					set.Add(cloneId);
					var cloneIndex = set.Sparse[cloneId];
					set.CopyDataAt(index, cloneIndex);
				}
			}

			return clone;
		}

		/// <summary>
		/// Destroys this entity.
		/// </summary>
		/// <returns>
		/// True if the entity was destroyed; false if it was already not alive.
		/// </returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool Destroy(this World world, Entity entity)
		{
			return world.Entities.Destroy(entity.Id);
		}

		/// <summary>
		/// Checks whether the entity is alive.
		/// </summary>
		/// <remarks>
		/// Throws if provided entity ID is negative.
		/// </remarks>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool IsAlive(this World world, Entity entity)
		{
			return world.Entities.IsAlive(entity);
		}

		/// <summary>
		/// Adds a component to the entity and sets its data.
		/// </summary>
		/// <remarks>
		/// Throws if the entity is not alive,
		/// or if the component has no associated data set.
		/// </remarks>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void Set<T>(this World world, Entity entity, T data)
		{
			InvalidSetOperationException.ThrowIfEntityDead(world.Entities, entity);

			var sparseSet = world.SparseSet<T>();

			NoDataException.ThrowIfHasNoData(sparseSet, typeof(T), DataAccessContext.WorldSet);

			var dataSet = (DataSet<T>)sparseSet;

			dataSet.Set(entity.Id, data);
		}

		/// <summary>
		/// Adds a component to the entity without initializing data.
		/// </summary>
		/// <returns>
		/// True if the component was added; false if it was already present.
		/// </returns>
		/// <remarks>
		/// Throws if the entity is not alive.
		/// </remarks>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool Add<T>(this World world, Entity entity)
		{
			InvalidAddOperationException.ThrowIfEntityDead(world.Entities, entity);

			return world.SparseSet<T>().Add(entity.Id);
		}

		/// <summary>
		/// Removes a component from the entity.
		/// </summary>
		/// <returns>
		/// True if the component was removed; false if it was not present.
		/// </returns>
		/// <remarks>
		/// Throws if the entity is not alive.
		/// </remarks>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool Remove<T>(this World world, Entity entity)
		{
			InvalidRemoveOperationException.ThrowIfEntityDead(world.Entities, entity);

			return world.SparseSet<T>().Remove(entity.Id);
		}

		/// <summary>
		/// Checks whether the entity has such a component.
		/// </summary>
		/// <remarks>
		/// Throws if the entity is not alive.
		/// </remarks>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool Has<T>(this World world, Entity entity)
		{
			InvalidHasOperationException.ThrowIfEntityDead(world.Entities, entity);

			return world.SparseSet<T>().Has(entity.Id);
		}

		/// <summary>
		/// Returns a reference to the component of the entity.
		/// </summary>
		/// <remarks>
		/// Throws if the entity is not alive,
		/// or if the component has no associated data set.
		/// </remarks>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ref T Get<T>(this World world, Entity entity)
		{
			InvalidGetOperationException.ThrowIfEntityDead(world.Entities, entity);

			var sparseSet = world.SparseSet<T>();

			NoDataException.ThrowIfHasNoData(sparseSet, typeof(T), DataAccessContext.WorldGet);

			var dataSet = (DataSet<T>)sparseSet;

			return ref dataSet.Get(entity.Id);
		}
	}
}
