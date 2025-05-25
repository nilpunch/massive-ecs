#if !MASSIVE_DISABLE_ASSERT
#define MASSIVE_ASSERT
#endif

using System.Runtime.CompilerServices;
using Unity.IL2CPP.CompilerServices;

namespace Massive
{
	[Il2CppSetOption(Option.NullChecks, false)]
	[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
	public static class WorldIdExtensions
	{
		/// <summary>
		/// Creates a unique entity and returns its ID.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int Create(this World world)
		{
			return world.Entities.Create().Id;
		}

		/// <summary>
		/// Creates a unique entity, adds a component without initializing data, and returns the entity ID.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int Create<T>(this World world)
		{
			var id = world.Create();
			world.Add<T>(id);
			return id;
		}

		/// <summary>
		/// Creates a unique entity, adds a component with provided data, and returns the entity ID.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int Create<T>(this World world, T data)
		{
			var id = world.Create();
			world.Set(id, data);
			return id;
		}

		/// <summary>
		/// Creates a unique entity with components of another entity and returns the entity ID.<br/>
		/// Makes shallow copy of each component.
		/// </summary>
		/// <remarks>
		/// Throws if the entity with this ID is not alive.
		/// </remarks>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int Clone(this World world, int id)
		{
			InvalidCloneOperationException.ThrowIfEntityDead(world.Entities, id);

			var cloneId = world.Create();

			var setList = world.Sets.AllSets;
			var setCount = setList.Count;
			var sets = setList.Items;
			for (var i = 0; i < setCount; i++)
			{
				var set = sets[i];
				var index = set.GetIndexOrNegative(id);
				if (index >= 0)
				{
					set.Add(cloneId);
					var cloneIndex = set.Sparse[cloneId];
					set.CopyDataAt(index, cloneIndex);
				}
			}

			return cloneId;
		}

		/// <summary>
		/// Destroys any alive entity with this ID.
		/// </summary>
		/// <returns>
		/// True if the entity was destroyed; false if it was already not alive.
		/// </returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool Destroy(this World world, int id)
		{
			return world.Entities.Destroy(id);
		}

		/// <summary>
		/// Checks whether the entity with this ID is alive.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool IsAlive(this World world, int id)
		{
			return world.Entities.IsAlive(id);
		}

		/// <summary>
		/// Adds a component to the entity with this ID and sets its data.
		/// </summary>
		/// <remarks>
		/// Throws if the entity with this ID is not alive,
		/// or if the component has no associated data set.
		/// </remarks>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void Set<T>(this World world, int id, T data)
		{
			InvalidSetOperationException.ThrowIfEntityDead(world.Entities, id);

			var sparseSet = world.SparseSet<T>();

			NoDataException.ThrowIfHasNoData(sparseSet, typeof(T), DataAccessContext.WorldSet);

			var dataSet = (DataSet<T>)sparseSet;

			dataSet.Set(id, data);
		}

		/// <summary>
		/// Adds a component to the entity with this ID without initializing data.
		/// </summary>
		/// <returns>
		/// True if the component was added; false if it was already present.
		/// </returns>
		/// <remarks>
		/// Throws if the entity with this ID is not alive.
		/// </remarks>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool Add<T>(this World world, int id)
		{
			InvalidAddOperationException.ThrowIfEntityDead(world.Entities, id);

			return world.SparseSet<T>().Add(id);
		}

		/// <summary>
		/// Removes a component from the entity with this ID.
		/// </summary>
		/// <returns>
		/// True if the component was removed; false if it was not present.
		/// </returns>
		/// <remarks>
		/// Throws if the entity with this ID is not alive.
		/// </remarks>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool Remove<T>(this World world, int id)
		{
			InvalidRemoveOperationException.ThrowIfEntityDead(world.Entities, id);

			return world.SparseSet<T>().Remove(id);
		}

		/// <summary>
		/// Checks whether the entity with this ID has such a component.
		/// </summary>
		/// <remarks>
		/// Throws if the entity with this ID is not alive.
		/// </remarks>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool Has<T>(this World world, int id)
		{
			InvalidHasOperationException.ThrowIfEntityDead(world.Entities, id);

			return world.SparseSet<T>().Has(id);
		}

		/// <summary>
		/// Returns a reference to the component of the entity with this ID.
		/// </summary>
		/// <remarks>
		/// Throws if the entity with this ID is not alive,
		/// or if the component has no associated data set.
		/// </remarks>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ref T Get<T>(this World world, int id)
		{
			InvalidGetOperationException.ThrowIfEntityDead(world.Entities, id);

			var sparseSet = world.SparseSet<T>();

			NoDataException.ThrowIfHasNoData(sparseSet, typeof(T), DataAccessContext.WorldGet);

			var dataSet = (DataSet<T>)sparseSet;

			return ref dataSet.Get(id);
		}
	}
}
