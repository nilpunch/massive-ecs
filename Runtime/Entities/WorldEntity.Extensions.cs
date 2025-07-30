#if !MASSIVE_DISABLE_ASSERT
#define MASSIVE_ASSERT
#endif

using System.Runtime.CompilerServices;

namespace Massive
{
	public readonly partial struct WorldEntity
	{
		/// <summary>
		/// Creates a unique entity with components of another entity.<br/>
		/// Makes shallow copy of each component.
		/// </summary>
		/// <remarks>
		/// Throws if the entity is not alive.
		/// </remarks>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Entity Clone()
		{
			InvalidCloneOperationException.ThrowIfEntityDead(World.Entities, Entity);

			var entityId = Id;
			var clone = World.CreateEntity();
			var cloneId = clone.Id;

			var setList = World.Sets.AllSets;
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
		public bool Destroy()
		{
			return World.Entities.Destroy(Id);
		}

		/// <summary>
		/// Adds a component to the entity and sets its data.
		/// </summary>
		/// <remarks>
		/// Throws if the entity is not alive,
		/// or if the component has no associated data set.
		/// </remarks>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Set<T>(T data)
		{
			InvalidSetOperationException.ThrowIfEntityDead(World.Entities, Entity);

			var info = TypeId<T>.Info;

			World.Sets.EnsureLookupAt(info.Index);
			var candidate = World.Sets.Lookup[info.Index];

			if (candidate == null)
			{
				candidate = World.Sets.Get<T>();
			}

			NoDataException.ThrowIfHasNoData(candidate, info.Type, DataAccessContext.WorldSet);

			var dataSet = (DataSet<T>)candidate;

			dataSet.Set(Id, data);
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
		public bool Add<T>()
		{
			InvalidAddOperationException.ThrowIfEntityDead(World.Entities, Entity);

			var info = TypeId<T>.Info;

			World.Sets.EnsureLookupAt(info.Index);
			var candidate = World.Sets.Lookup[info.Index];

			if (candidate == null)
			{
				candidate = World.Sets.Get<T>();
			}

			return candidate.Add(Id);
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
		public bool Remove<T>()
		{
			InvalidRemoveOperationException.ThrowIfEntityDead(World.Entities, Entity);

			var info = TypeId<T>.Info;

			World.Sets.EnsureLookupAt(info.Index);
			var candidate = World.Sets.Lookup[info.Index];

			if (candidate == null)
			{
				candidate = World.Sets.Get<T>();
			}

			return candidate.Remove(Id);
		}

		/// <summary>
		/// Checks whether the entity has such a component.
		/// </summary>
		/// <remarks>
		/// Throws if the entity is not alive.
		/// </remarks>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool Has<T>()
		{
			InvalidHasOperationException.ThrowIfEntityDead(World.Entities, Entity);

			var info = TypeId<T>.Info;

			World.Sets.EnsureLookupAt(info.Index);
			var candidate = World.Sets.Lookup[info.Index];

			if (candidate == null)
			{
				candidate = World.Sets.Get<T>();
			}

			return candidate.Has(Id);
		}

		/// <summary>
		/// Returns a reference to the component of the entity.
		/// </summary>
		/// <remarks>
		/// Throws if the entity is not alive,
		/// or if the component has no associated data set.
		/// </remarks>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public ref T Get<T>()
		{
			InvalidGetOperationException.ThrowIfEntityDead(World.Entities, Entity);

			var info = TypeId<T>.Info;

			World.Sets.EnsureLookupAt(info.Index);
			var candidate = World.Sets.Lookup[info.Index];

			if (candidate == null)
			{
				candidate = World.Sets.Get<T>();
			}

			NoDataException.ThrowIfHasNoData(candidate, info.Type, DataAccessContext.WorldGet);

			var dataSet = (DataSet<T>)candidate;

			return ref dataSet.Get(Id);
		}
	}
}
