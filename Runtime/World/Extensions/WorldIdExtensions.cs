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
		/// Creates a unique entity with the assigned component and returns the entity ID.
		/// Does not initialize component data.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int Create<T>(this World world)
		{
			var id = world.Create();
			world.Assign<T>(id);
			return id;
		}

		/// <summary>
		/// Creates a unique entity with the assigned component and returns the entity ID.
		/// </summary>
		/// <param name="data"> Initial data for the assigned component. </param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int Create<T>(this World world, T data)
		{
			var id = world.Create();
			world.Assign(id, data);
			return id;
		}

		/// <summary>
		/// Creates a unique entity with components of another entity and returns the entity ID.
		/// </summary>
		/// <remarks>
		/// Cloning entity that is not alive will throw an exception.
		/// </remarks>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int Clone(this World world, int id)
		{
			Assert.IsAlive(world, id);

			var cloneId = world.Create();

			var setList = world.SetRegistry.All;
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
		/// <remarks>
		/// Will throw an exception if the entity with this ID is not alive.
		/// </remarks>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void Destroy(this World world, int id)
		{
			Assert.IsAlive(world, id);

			world.Entities.Destroy(id);
		}

		/// <summary>
		/// Checks whether the entity with this ID is alive, regardless of version.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool IsAlive(this World world, int id)
		{
			return world.Entities.IsAlive(id);
		}

		/// <summary>
		/// Assigns a component to the entity with this ID, regardless of version.
		/// Repeat assignments are allowed.
		/// </summary>
		/// <param name="data"> Initial data for the assigned component. </param>
		/// <remarks>
		/// Will throw an exception if the entity with this ID is not alive.
		/// </remarks>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void Assign<T>(this World world, int id, T data)
		{
			Assert.IsAlive(world, id);

			var set = world.Set<T>();
			set.Assign(id);
			if (set is DataSet<T> dataSet)
			{
				dataSet.Get(id) = data;
			}
		}

		/// <summary>
		/// Assigns a component to the entity with this ID, regardless of version, without data initialization.
		/// Repeat assignments are allowed.
		/// </summary>
		/// <remarks>
		/// Will throw an exception if the entity with this ID is not alive.
		/// </remarks>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void Assign<T>(this World world, int id)
		{
			Assert.IsAlive(world, id);

			world.Set<T>().Assign(id);
		}

		/// <summary>
		/// Unassigns a component from the entity with this ID, regardless of version.
		/// Repeat unassignments are allowed.
		/// </summary>
		/// <remarks>
		/// Will throw an exception if the entity with this ID is not alive.
		/// </remarks>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void Unassign<T>(this World world, int id)
		{
			Assert.IsAlive(world, id);

			world.Set<T>().Unassign(id);
		}

		/// <summary>
		/// Checks whether the entity with this ID has such a component, regardless of version.
		/// </summary>
		/// <remarks>
		/// Will throw an exception if the entity with this ID is not alive.
		/// </remarks>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool Has<T>(this World world, int id)
		{
			Assert.IsAlive(world, id);

			return world.Set<T>().IsAssigned(id);
		}

		/// <summary>
		/// Returns a reference to the component of the entity with this ID, regardless of version.
		/// </summary>
		/// <remarks>
		/// Requesting a component from the entity that is being destroyed will throw an exception,
		/// and this method will throw an exception if the type has no associated data.
		/// </remarks>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ref T Get<T>(this World world, int id)
		{
			Assert.IsAlive(world, id);
			Assert.TypeHasData<T>(world, SuggestionMessage.DontUseGetWithEmptyTypes);

			var dataSet = world.Set<T>() as DataSet<T>;

			return ref dataSet.Get(id);
		}
	}
}
