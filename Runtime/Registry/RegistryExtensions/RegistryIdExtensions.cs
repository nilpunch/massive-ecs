using System;
using System.Runtime.CompilerServices;

namespace Massive
{
	public static class RegistryIdExtensions
	{
		/// <summary>
		/// Creates a unique entity and returns its ID.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int Create(this Registry registry)
		{
			return registry.Entities.Create().Id;
		}

		/// <summary>
		/// Creates a unique entity with the assigned component and returns the entity ID.
		/// </summary>
		/// <param name="data"> Initial data for the assigned component. </param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int Create<T>(this Registry registry, T data = default)
		{
			var id = registry.Create();
			registry.Assign(id, data);
			return id;
		}

		/// <summary>
		/// Creates a unique entity with components of another entity and returns the entity ID.
		/// </summary>
		/// <remarks>
		/// Cloning entity that is not alive will throw an exception.
		/// </remarks>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int Clone(this Registry registry, int id)
		{
			if (!registry.IsAlive(id))
			{
				throw new Exception("The entity you want to clone is not alive!");
			}

			var cloneId = registry.Create();

			var sets = registry.BitsetSet.GetAllBits(id);
			for (var i = 0; i < sets.Length; i++)
			{
				var set = registry.SetRegistry.FindSetById(sets[i]);
				set.Assign(cloneId);
				if (set is IDataSet dataSet)
				{
					dataSet.CopyData(id, cloneId);
				}
			}

			return cloneId;
		}

		/// <summary>
		/// Destroys any alive entity with this ID, regardless of generation.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void Destroy(this Registry registry, int id)
		{
			registry.Entities.Destroy(id);
		}

		/// <summary>
		/// Checks whether an entity with this ID is alive, regardless of generation.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool IsAlive(this Registry registry, int id)
		{
			return registry.Entities.IsAlive(id);
		}

		/// <summary>
		/// Assigns a component to an entity with this ID, regardless of generation.
		/// </summary>
		/// <param name="data"> Initial data for the assigned component. </param>
		/// <remarks>
		/// Assigning a component to an entity that is being destroyed can lead to undefined behavior.
		/// </remarks>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void Assign<T>(this Registry registry, int id, T data = default)
		{
			var set = registry.Set<T>();
			if (set is DataSet<T> dataSet)
			{
				dataSet.Assign(id, data);
			}
			else
			{
				set.Assign(id);
			}
		}

		/// <summary>
		/// Unassigns a component from an entity with this ID, regardless of generation.
		/// </summary>
		/// <remarks>
		/// Unassigning a component from an entity that is being destroyed can lead to undefined behavior.
		/// </remarks>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void Unassign<T>(this Registry registry, int id)
		{
			registry.Set<T>().Unassign(id);
		}

		/// <summary>
		/// Checks whether an entity with this ID has such a component, regardless of generation.
		/// </summary>
		/// <remarks>
		/// Returns false if the entity is not alive.
		/// </remarks>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool Has<T>(this Registry registry, int id)
		{
			return registry.Set<T>().IsAssigned(id);
		}

		/// <summary>
		/// Returns a reference to the component of the entity with this ID, regardless of generation.
		/// </summary>
		/// <remarks>
		/// Requesting a component from an entity that is being destroyed can lead to undefined behavior,
		/// and this method may throw an exception if the type has no associated data.
		/// </remarks>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ref T Get<T>(this Registry registry, int id)
		{
			if (registry.Set<T>() is not DataSet<T> dataSet)
			{
				throw new Exception("Type has no associated data!");
			}

			return ref dataSet.Get(id);
		}
	}
}
