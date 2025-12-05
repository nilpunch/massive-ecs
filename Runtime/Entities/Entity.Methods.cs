#if !MASSIVE_DISABLE_ASSERT
#define MASSIVE_ASSERT
#endif

using System.Runtime.CompilerServices;
using Unity.IL2CPP.CompilerServices;

namespace Massive
{
	[Il2CppSetOption(Option.NullChecks, false)]
	[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
	public partial struct Entity
	{
		/// <summary>
		/// Creates a unique entity with components of another entity.<br/>
		/// Makes shallow copy of each component.
		/// </summary>
		/// <remarks>
		/// Throws if the entity is not alive.
		/// </remarks>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public readonly Entity Clone()
		{
			InvalidCloneOperationException.ThrowIfEntityDead(this);

			var entityId = Id;
			var clone = World.CreateEntity();
			var cloneId = clone.Id;

			var sets = World.Sets;
			var buffer = World.Components.Buffer;
			var componentCount = World.Components.GetAll(entityId, buffer);

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
		public readonly bool Destroy()
		{
			return World != null && World.Entities.Destroy(Id);
		}

		/// <summary>
		/// Adds a component to the entity and sets its data.
		/// </summary>
		/// <remarks>
		/// Throws if the entity is not alive,
		/// or if the component has no associated data set.
		/// </remarks>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public readonly void Set<T>(T data)
		{
			InvalidSetOperationException.ThrowIfEntityDead(this);

			var info = TypeId<SetKind, T>.Info;

			World.Sets.EnsureLookupByTypeAt(info.Index);
			var candidate = World.Sets.LookupByTypeId[info.Index];

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
		public readonly bool Add<T>()
		{
			InvalidAddOperationException.ThrowIfEntityDead(this);

			var info = TypeId<SetKind, T>.Info;

			World.Sets.EnsureLookupByTypeAt(info.Index);
			var candidate = World.Sets.LookupByTypeId[info.Index];

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
		public readonly bool Remove<T>()
		{
			InvalidRemoveOperationException.ThrowIfEntityDead(this);

			var info = TypeId<SetKind, T>.Info;

			World.Sets.EnsureLookupByTypeAt(info.Index);
			var candidate = World.Sets.LookupByTypeId[info.Index];

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
		public readonly bool Has<T>()
		{
			InvalidHasOperationException.ThrowIfEntityDead(this);

			var info = TypeId<SetKind, T>.Info;

			World.Sets.EnsureLookupByTypeAt(info.Index);
			var candidate = World.Sets.LookupByTypeId[info.Index];

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
		public readonly ref T Get<T>()
		{
			InvalidGetOperationException.ThrowIfEntityDead(this);

			var info = TypeId<SetKind, T>.Info;

			World.Sets.EnsureLookupByTypeAt(info.Index);
			var candidate = World.Sets.LookupByTypeId[info.Index];

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
