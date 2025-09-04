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
		private const int EndHoleId = int.MaxValue;

		/// <summary>
		/// Gets the current number of elements in the packed array.<br/>
		/// May include removed IDs due to holes.
		/// </summary>
		public int Count { get; protected set; }

		/// <summary>
		/// The packed array, containing IDs.<br/>
		/// Don't cache it and use as is, underlying array can be resized at any moment.
		/// </summary>
		public int[] Packed { get; protected set; } = Array.Empty<int>();
		
		/// <summary>
		/// The sparse array, containing entities versions.<br/>
		/// Don't cache it and use as is, underlying array can be resized at any moment.
		/// </summary>
		public uint[] Versions { get; private set; } = Array.Empty<uint>();

		/// <summary>
		/// The sparse array, mapping IDs to their packed indices.<br/>
		/// Don't cache it and use as is, underlying array can be resized at any moment.
		/// </summary>
		public int[] Sparse { get; private set; } = Array.Empty<int>();

		/// <summary>
		/// The current capacity of the packed array.
		/// </summary>
		public int PackedCapacity { get; private set; }

		/// <summary>
		/// The current capacity of the sparse array.
		/// </summary>
		public int SparseCapacity { get; private set; }

		/// <summary>
		/// The maximum count of entity ids in use.
		/// </summary>
		public int UsedIds { get; private set; }

		/// <summary>
		/// The ID of the next available hole in the sparse array, or <see cref="EndHoleId"/> if no holes exist.
		/// </summary>
		private int NextHoleId { get; set; } = EndHoleId;

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
		/// Gets or sets the current state for serialization or rollback purposes.
		/// </summary>
		public State CurrentState
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => new State(Count, UsedIds, NextHoleId);
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			set
			{
				Count = value.Count;
				UsedIds = value.UsedIds;
				NextHoleId = value.NextHoleId;
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Entifier Create()
		{
			EnsureCapacityAt(Count);

			Entifier entifier;

			if (NextHoleId != EndHoleId)
			{
				var id = NextHoleId;
				var index = Sparse[id];
				NextHoleId = ~Packed[index];
				Packed[index] = id;
				entifier = new Entifier(id, Versions[id]);
			}
			else if (Count < UsedIds)
			{
				var id = Packed[Count];
				entifier = new Entifier(id, Versions[id]);
				Count += 1;
			}
			else
			{
				entifier = new Entifier(UsedIds, 1U);
				AssignEntity(UsedIds, 1U, Count);
				UsedIds += 1;
				Count += 1;
			}

			AddInternal(entifier.Id);
			AfterCreated?.Invoke(entifier.Id);

			return entifier;
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
			if (id >= UsedIds || Sparse[id] >= Count || Packed[Sparse[id]] != id)
			{
				return false;
			}

			BeforeDestroyed?.Invoke(id);
			WorldContext?.EntityDestroyed(id);

			var index = Sparse[id];

			Packed[index] = ~NextHoleId;
			MathUtils.IncrementWrapTo1(ref Versions[id]);
			NextHoleId = id;

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
			EnsureCapacityAt(needToCreate + Count);

			while (NextHoleId != EndHoleId && needToCreate > 0)
			{
				needToCreate -= 1;
				var id = NextHoleId;
				var index = Sparse[id];
				NextHoleId = ~Packed[index];
				Packed[index] = id;
				AddInternal(id);
				AfterCreated?.Invoke(id);
			}

			while (Count < UsedIds && needToCreate > 0)
			{
				needToCreate -= 1;
				Count += 1;
				AddInternal(Packed[Count - 1]);
				AfterCreated?.Invoke(Packed[Count - 1]);
			}

			for (var i = 0; i < needToCreate; i++)
			{
				AssignEntity(UsedIds, 1U, Count);
				UsedIds += 1;
				Count += 1;
				AddInternal(Packed[Count - 1]);
				AfterCreated?.Invoke(UsedIds - 1);
			}
		}

		/// <summary>
		/// Destroys all entities and triggers the <see cref="BeforeDestroyed"/> event for each one.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Clear()
		{
			for (var i = Count - 1; i >= 0; i--)
			{
				var id = Packed[i];
				if (id >= 0)
				{
					BeforeDestroyed?.Invoke(id);
					WorldContext?.EntityDestroyed(id);
					MathUtils.IncrementWrapTo1(ref Versions[id]);
				}
				Count -= 1;
			}

			var nextHoleId = NextHoleId;
			while (nextHoleId != EndHoleId)
			{
				var holeId = nextHoleId;
				var holeIndex = Sparse[holeId];
				nextHoleId = ~Packed[holeIndex];
				Packed[holeIndex] = holeId;
			}
			NextHoleId = EndHoleId;
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

			var index = Sparse[entifier.Id];

			return index < Count && Packed[index] == entifier.Id && Versions[entifier.Id] == entifier.Version;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool IsAlive(int id)
		{
			return id >= 0 && id < UsedIds && Sparse[id] < Count && Packed[Sparse[id]] == id;
		}

		/// <summary>
		/// Ensures the sparse and packed arrays has sufficient capacity for the specified index.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void EnsureCapacityAt(int index)
		{
			if (index >= SparseCapacity)
			{
				var newCapacity = MathUtils.NextPowerOf2(index + 1);
				ResizePacked(newCapacity);
				ResizeSparse(newCapacity);
				WorldContext?.Masks.EnsureEntitiesCapacity(newCapacity);
			}
		}

		/// <summary>
		/// Resizes the packed array to the specified capacity.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void ResizePacked(int capacity)
		{
			Packed = Packed.Resize(capacity);
			PackedCapacity = capacity;
		}

		/// <summary>
		/// Resizes the sparse array to the specified capacity.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void ResizeSparse(int capacity)
		{
			Sparse = Sparse.Resize(capacity);
			Versions = Versions.Resize(capacity);
			if (capacity > SparseCapacity)
			{
				Array.Fill(Versions, 1U, SparseCapacity, capacity - SparseCapacity);
			}
			SparseCapacity = capacity;
		}

		/// <summary>
		/// Removes all holes from the packed array.
		/// </summary>
		public void Compact()
		{
			if (NextHoleId != EndHoleId)
			{
				var count = Count;
				var nextHoleId = NextHoleId;

				for (; count > 0 && Packed[count - 1] < 0; count--) { }

				while (nextHoleId != EndHoleId)
				{
					var holeId = nextHoleId;
					var holeIndex = Sparse[holeId];
					nextHoleId = ~Packed[holeIndex];
					if (holeIndex < count)
					{
						count -= 1;

						var holeVersion = Versions[holeId];
						var id = Packed[count];
						AssignEntity(id, Versions[id], holeIndex);
						AssignEntity(holeId, holeVersion, count);

						for (; count > 0 && Packed[count - 1] < 0; count--) { }
					}
					else
					{
						Packed[holeIndex] = holeId;
					}
				}

				Count = count;
				NextHoleId = EndHoleId;
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public BitsEnumerator GetEnumerator()
		{
			var bits = BitsPool.RentClone(this);
			PushRemoveOnRemove(bits);
			var pops = PopsPool.Rent().AddPopOnRemove(this);
			return new BitsEnumerator(bits, pops, Bits1.Length);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void AssignEntity(int id, uint version, int index)
		{
			Sparse[id] = index;
			Packed[index] = id;
			Versions[id] = version;
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

			Array.Copy(Packed, other.Packed, UsedIds);
			Array.Copy(Versions, other.Versions, UsedIds);
			Array.Copy(Sparse, other.Sparse, UsedIds);

			if (UsedIds < other.UsedIds)
			{
				Array.Fill(other.Versions, 1U, UsedIds, other.UsedIds - UsedIds);
			}

			other.CurrentState = CurrentState;
		}

		public readonly struct State
		{
			public readonly int Count;
			public readonly int UsedIds;
			public readonly int NextHoleId;

			public State(int count, int usedIds, int nextHoleId)
			{
				Count = count;
				UsedIds = usedIds;
				NextHoleId = nextHoleId;
			}
		}
	}
}
