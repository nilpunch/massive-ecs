#if !MASSIVE_DISABLE_ASSERT
#define MASSIVE_ASSERT
#endif

using System.Runtime.CompilerServices;
using Unity.IL2CPP.CompilerServices;

namespace Massive
{
	[Il2CppEagerStaticClassConstruction]
	[Il2CppSetOption(Option.NullChecks, false)]
	[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
	public static class WorldEntifierExtensions
	{
		/// <summary>
		/// Returns entity identifier for this ID.
		/// </summary>
		/// <remarks>
		/// Throws if the entity with this ID is not alive.
		/// </remarks>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Entifier GetEntifier(this World world, int id)
		{
			return world.Entities.GetEntifier(id);
		}

		/// <summary>
		/// Creates a unique entity and returns the identifier.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Entifier CreateEntifier(this World world)
		{
			return world.Entities.Create();
		}

		/// <summary>
		/// Creates a unique entity, adds a component without initializing data, and returns the identifier.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Entifier CreateEntifier<T>(this World world)
		{
			var entity = world.CreateEntifier();
			world.Add<T>(entity.Id);
			return entity;
		}

		/// <summary>
		/// Creates a unique entity, adds a component with provided data, and returns the identifier.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Entifier CreateEntifier<T>(this World world, T data)
		{
			var entity = world.CreateEntifier();
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
		public static Entifier Clone(this World world, Entifier entifier)
		{
			InvalidCloneOperationException.ThrowIfEntityDead(world.Entities, entifier);

			var entityId = entifier.Id;
			var clone = world.Entities.Create();
			var cloneId = clone.Id;

			var sets = world.Sets;
			var buffer = world.Components.Buffer;
			var componentCount = world.Components.GetAll(entityId, buffer);

			for (var i = 0; i < componentCount; i++)
			{
				var set = sets.LookupByComponentId[buffer[i]];
				set.Add(cloneId);
				set.CopyData(entityId, cloneId);
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
		public static bool Destroy(this World world, Entifier entifier)
		{
			return world.Entities.Destroy(entifier.Id);
		}

		/// <summary>
		/// Checks whether the entity is alive.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool IsAlive(this World world, Entifier entifier)
		{
			return world.Entities.IsAlive(entifier);
		}

		/// <summary>
		/// Adds a component to the entity and sets its data.
		/// </summary>
		/// <remarks>
		/// Throws if the entity is not alive,
		/// or if the component has no associated data set.
		/// </remarks>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void Set<T>(this World world, Entifier entifier, T data)
		{
			InvalidSetOperationException.ThrowIfEntityDead(world.Entities, entifier);

			var info = TypeId<SetKind, T>.Info;

			world.Sets.EnsureLookupByTypeAt(info.Index);
			var candidate = world.Sets.LookupByTypeId[info.Index];

			if (candidate == null)
			{
				candidate = world.Sets.Get<T>();
			}

			NoDataException.ThrowIfHasNoData(candidate, info.Type, DataAccessContext.WorldSet);

			var dataSet = (DataSet<T>)candidate;

			dataSet.Set(entifier.Id, data);
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
		public static bool Add<T>(this World world, Entifier entifier)
		{
			InvalidAddOperationException.ThrowIfEntityDead(world.Entities, entifier);

			var info = TypeId<SetKind, T>.Info;

			world.Sets.EnsureLookupByTypeAt(info.Index);
			var candidate = world.Sets.LookupByTypeId[info.Index];

			if (candidate == null)
			{
				candidate = world.Sets.Get<T>();
			}

			return candidate.Add(entifier.Id);
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
		public static bool Remove<T>(this World world, Entifier entifier)
		{
			InvalidRemoveOperationException.ThrowIfEntityDead(world.Entities, entifier);

			var info = TypeId<SetKind, T>.Info;

			world.Sets.EnsureLookupByTypeAt(info.Index);
			var candidate = world.Sets.LookupByTypeId[info.Index];

			if (candidate == null)
			{
				candidate = world.Sets.Get<T>();
			}

			return candidate.Remove(entifier.Id);
		}

		/// <summary>
		/// Checks whether the entity has such a component.
		/// </summary>
		/// <remarks>
		/// Throws if the entity is not alive.
		/// </remarks>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool Has<T>(this World world, Entifier entifier)
		{
			InvalidHasOperationException.ThrowIfEntityDead(world.Entities, entifier);

			var info = TypeId<SetKind, T>.Info;

			world.Sets.EnsureLookupByTypeAt(info.Index);
			var candidate = world.Sets.LookupByTypeId[info.Index];

			if (candidate == null)
			{
				candidate = world.Sets.Get<T>();
			}

			return candidate.Has(entifier.Id);
		}

		/// <summary>
		/// Returns a reference to the component of the entity.
		/// </summary>
		/// <remarks>
		/// Throws if the entity is not alive,
		/// or if the component has no associated data set.
		/// </remarks>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ref T Get<T>(this World world, Entifier entifier)
		{
			InvalidGetOperationException.ThrowIfEntityDead(world.Entities, entifier);

			var info = TypeId<SetKind, T>.Info;

			world.Sets.EnsureLookupByTypeAt(info.Index);
			var candidate = world.Sets.LookupByTypeId[info.Index];

			if (candidate == null)
			{
				candidate = world.Sets.Get<T>();
			}

			NoDataException.ThrowIfHasNoData(candidate, info.Type, DataAccessContext.WorldGet);

			var dataSet = (DataSet<T>)candidate;

			return ref dataSet.Get(entifier.Id);
		}
	}
}
