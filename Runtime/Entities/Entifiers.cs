#if !MASSIVE_DISABLE_ASSERT
#define MASSIVE_ASSERT
#endif

using System;
using System.Runtime.CompilerServices;
using Unity.IL2CPP.CompilerServices;

namespace Massive
{
	[Il2CppSetOption(Option.NullChecks, false)]
	[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
	public class Entifiers : BitsBase
	{
		public int[] Reuse { get; protected set; } = Array.Empty<int>();

		public int ReusedIds { get; protected set; }

		/// <summary>
		/// The sparse array, containing entities versions.<br/>
		/// </summary>
		public uint[] Versions { get; private set; } = Array.Empty<uint>();

		/// <summary>
		/// The current capacity of the versions array.
		/// </summary>
		public int VersionsCapacity { get; private set; }

		/// <summary>
		/// The maximum count of entity ids in use.
		/// </summary>
		public int UsedIds { get; private set; }

		/// <summary>
		/// Shortcut to access world.
		/// </summary>
		public WorldContext? WorldContext { get; set; }

		/// <summary>
		/// Shoots after entity is created.
		/// </summary>
		public event Action<int> AfterCreated;

		/// <summary>
		/// Shoots before entity is destroyed.
		/// </summary>
		public event Action<int> BeforeDestroyed;

		/// <summary>
		/// The current amount of alive entities.<br/>
		/// </summary>
		public int Count => UsedIds - ReusedIds;

		/// <summary>
		/// Gets or sets the current state for serialization or rollback purposes.
		/// </summary>
		public State CurrentState
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => new State(ReusedIds, UsedIds);
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			set
			{
				ReusedIds = value.ReusedIds;
				UsedIds = value.UsedIds;
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Entifier Create()
		{
			int id;
			uint version;

			if (ReusedIds != 0)
			{
				id = Reuse[--ReusedIds];
				version = Versions[id];
			}
			else
			{
				EnsureCapacityAt(UsedIds);
				id = UsedIds++;
				version = 1U;
			}

			SetBitInternal(id);
			AfterCreated?.Invoke(id);

			return new Entifier(id, version);
		}

		/// <summary>
		/// Destroys any alive entity with this ID. If the entity is already not alive, no action is performed.
		/// </summary>
		/// <returns>
		/// True if the entity was destroyed; false if it was already not alive.
		/// </returns>
		/// <remarks>
		/// Throws if provided ID is negative.
		/// </remarks>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool Destroy(int id)
		{
			NegativeArgumentException.ThrowIfNegative(id);

			// If entity is not alive, nothing to be done.
			if (id >= UsedIds || (Bits0[id >> 6] & (1UL << (id & 63))) == 0UL)
			{
				return false;
			}

			BeforeDestroyed?.Invoke(id);
			RemoveBitInternal(id);
			WorldContext?.EntityDestroyed(id);

			EnsureReuseAt(ReusedIds);
			Reuse[ReusedIds++] = id;
			MathUtils.IncrementWrapTo1(ref Versions[id]);

			return true;
		}

		/// <remarks>
		/// Throws if provided amount is negative.
		/// </remarks>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void CreateMany(int amount)
		{
			NegativeArgumentException.ThrowIfNegative(amount);

			var needToCreate = amount;
			EnsureCapacityAt(needToCreate + UsedIds);

			while (ReusedIds != 0 && needToCreate > 0)
			{
				needToCreate -= 1;
				var id = Reuse[--ReusedIds];
				SetBitInternal(id);
				AfterCreated?.Invoke(id);
			}

			for (var i = 0; i < needToCreate; i++)
			{
				var id = UsedIds++;
				SetBitInternal(id);
				AfterCreated?.Invoke(id);
			}
		}

		/// <summary>
		/// Destroys all entities and triggers the <see cref="BeforeDestroyed"/> event for each one.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Clear()
		{
			throw new NotImplementedException();
		}

		/// <remarks>
		/// Throws if the entity with this ID is not alive.
		/// </remarks>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Entifier GetEntifier(int id)
		{
			EntityNotAliveException.ThrowIfEntityDead(this, id);

			return new Entifier(id, Versions[id]);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool IsAlive(Entifier entifier)
		{
			if (entifier.Id < 0 || entifier.Id >= UsedIds)
			{
				return false;
			}

			return Versions[entifier.Id] == entifier.Version;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool IsAlive(int id)
		{
			return id >= 0 && id < UsedIds && (Bits0[id >> 6] & (1UL << (id & 63))) != 0UL;
		}

		/// <summary>
		/// Ensures the sparse and packed arrays has sufficient capacity for the specified index.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void EnsureCapacityAt(int index)
		{
			if (index >= VersionsCapacity)
			{
				var newCapacity = MathUtils.NextPowerOf2(index + 1);
				ResizeVersions(newCapacity);
				WorldContext?.Components.EnsureEntitiesCapacity(newCapacity);
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void EnsureReuseAt(int index)
		{
			if (index >= Reuse.Length) 
			{
				Reuse = Reuse.ResizeToNextPowOf2(index + 1);
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void ResizeVersions(int capacity)
		{
			Versions = Versions.Resize(capacity);
			if (capacity > VersionsCapacity)
			{
				Array.Fill(Versions, 1U, VersionsCapacity, capacity - VersionsCapacity);
			}
			VersionsCapacity = capacity;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public BitsEnumerator GetEnumerator()
		{
			var bits = BitsPool.RentClone(this);
			PushRemoveOnRemove(bits);
			var pops = PopsPool.Rent().AddPopOnRemove(this);
			return new BitsEnumerator(bits, pops, Bits1.Length);
		}

		/// <summary>
		/// Creates and returns a new entities collection that is an exact copy of this one.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Entifiers Clone()
		{
			var clone = new Entifiers();
			CopyTo(clone);
			return clone;
		}

		/// <summary>
		/// Copies all entities and state from this entities collection into the specified one.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void CopyTo(Entifiers other)
		{
			CopyBitsTo(other);

			other.EnsureCapacityAt(UsedIds - 1);
			other.EnsureReuseAt(ReusedIds - 1);

			Array.Copy(Reuse, other.Reuse, ReusedIds);
			Array.Copy(Versions, other.Versions, UsedIds);

			if (UsedIds < other.UsedIds)
			{
				Array.Fill(other.Versions, 1U, UsedIds, other.UsedIds - UsedIds);
			}

			other.CurrentState = CurrentState;
		}

		public readonly struct State
		{
			public readonly int ReusedIds;
			public readonly int UsedIds;

			public State(int reusedIds, int usedIds)
			{
				ReusedIds = reusedIds;
				UsedIds = usedIds;
			}
		}
	}
}
