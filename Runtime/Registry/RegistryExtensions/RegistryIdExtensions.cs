using System.Runtime.CompilerServices;
using Unity.IL2CPP.CompilerServices;

namespace Massive
{
	[Il2CppSetOption(Option.NullChecks, false)]
	[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
	[Il2CppSetOption(Option.DivideByZeroChecks, false)]
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
		/// Does not initialize component data.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int Create<T>(this Registry registry)
		{
			var id = registry.Create();
			registry.Assign<T>(id);
			return id;
		}

		/// <summary>
		/// Creates a unique entity with the assigned component and returns the entity ID.
		/// </summary>
		/// <param name="data"> Initial data for the assigned component. </param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int Create<T>(this Registry registry, T data)
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
			Debug.AssertEntityAlive(registry, id);

			var cloneId = registry.Create();

			var setList = registry.SetRegistry.All;
			var setCount = setList.Count;
			var sets = setList.Items;
			for (var i = 0; i < setCount; i++)
			{
				var set = sets[i];
				var index = set.GetIndexOrNegative(id);
				if (index >= 0)
				{
					set.Assign(cloneId);
					var cloneIndex = set.Sparse[cloneId];
					set.CopyDataAt(index, cloneIndex);
				}
			}

			return cloneId;
		}

		/// <summary>
		/// Destroys any alive entity with this ID, regardless of version.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void Destroy(this Registry registry, int id)
		{
			registry.Entities.Destroy(id);
		}

		/// <summary>
		/// Checks whether an entity with this ID is alive, regardless of version.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool IsAlive(this Registry registry, int id)
		{
			return registry.Entities.IsAlive(id);
		}

		/// <summary>
		/// Assigns a component to an entity with this ID, regardless of version.
		/// Repeat assignments are allowed.
		/// </summary>
		/// <param name="data"> Initial data for the assigned component. </param>
		/// <remarks>
		/// Assigning a component to an entity that is being destroyed will throw an exception.
		/// </remarks>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void Assign<T>(this Registry registry, int id, T data)
		{
			Debug.AssertEntityAlive(registry, id);

			var set = registry.Set<T>();
			set.Assign(id);
			if (set is DataSet<T> dataSet)
			{
				dataSet.Get(id) = data;
			}
		}

		/// <summary>
		/// Assigns a component to an entity with this ID, regardless of version, without data initialization.
		/// Repeat assignments are allowed.
		/// </summary>
		/// <remarks>
		/// Assigning a component to an entity that is being destroyed will throw an exception.
		/// </remarks>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void Assign<T>(this Registry registry, int id)
		{
			Debug.AssertEntityAlive(registry, id);

			registry.Set<T>().Assign(id);
		}

		/// <summary>
		/// Unassigns a component from an entity with this ID, regardless of version.
		/// </summary>
		/// <remarks>
		/// If the entity is not alive or does not have the component, nothing happens.
		/// </remarks>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void Unassign<T>(this Registry registry, int id)
		{
			registry.Set<T>().Unassign(id);
		}

		/// <summary>
		/// Checks whether an entity with this ID has such a component, regardless of version.
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
		/// Returns a reference to the component of the entity with this ID, regardless of version.
		/// </summary>
		/// <remarks>
		/// Requesting a component from an entity that is being destroyed will throw an exception,
		/// and this method will throw an exception if the type has no associated data.
		/// </remarks>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ref T Get<T>(this Registry registry, int id)
		{
			Debug.AssertEntityAlive(registry, id);
			Debug.AssertTypeHasData<T>(registry, SuggestionMessage.DontUseGetWithEmptyTypes);

			var dataSet = registry.Set<T>() as DataSet<T>;

			return ref dataSet.Get(id);
		}
	}
}
