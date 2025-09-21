﻿#if !MASSIVE_DISABLE_ASSERT
#define MASSIVE_ASSERT
#endif

using System;
using System.Runtime.CompilerServices;
using Unity.IL2CPP.CompilerServices;

namespace Massive
{
	[Il2CppSetOption(Option.NullChecks, false)]
	[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
	public class Entities : BitSetBase
	{
		public int[] Pool { get; protected set; } = Array.Empty<int>();

		public int PooledIds { get; protected set; }

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
		public World World { get; }

		private Sets Sets { get; }

		private Components Components { get; }

		private Allocators Allocators { get; }

		public Entities(World world = default)
		{
			World = world ?? new World();
			Sets = World.Sets;
			Components = World.Components;
			Allocators = World.Allocators;
		}

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
		public int Count => UsedIds - PooledIds;

		/// <summary>
		/// Gets or sets the current state for serialization or rollback purposes.
		/// </summary>
		public State CurrentState
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => new State(PooledIds, UsedIds);
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			set
			{
				PooledIds = value.ReusedIds;
				UsedIds = value.UsedIds;
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Entifier Create()
		{
			int id;
			uint version;

			if (PooledIds != 0)
			{
				id = Pool[--PooledIds];
				version = Versions[id];
			}
			else
			{
				EnsureEntityAt(UsedIds);
				id = UsedIds++;
				version = 1U;
			}

			SetBit(id);
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
			if (id >= UsedIds || (Bits[id >> 6] & (1UL << (id & 63))) == 0UL)
			{
				return false;
			}

			BeforeDestroyed?.Invoke(id);
			RemoveBit(id);
			DestroyInWorld(id);

			EnsurePoolAt(PooledIds);
			Pool[PooledIds++] = id;
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
			EnsureEntityAt(needToCreate + UsedIds);

			while (PooledIds != 0 && needToCreate > 0)
			{
				needToCreate -= 1;
				var id = Pool[--PooledIds];
				SetBit(id);
				AfterCreated?.Invoke(id);
			}

			for (var i = 0; i < needToCreate; i++)
			{
				var id = UsedIds++;
				SetBit(id);
				AfterCreated?.Invoke(id);
			}
		}

		/// <summary>
		/// Destroys all entities and triggers the <see cref="BeforeDestroyed"/> event for each one.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Clear()
		{
			var blocksLength = NonEmptyBlocks.Length;

			var deBruijn = MathUtils.DeBruijn;
			for (var blockIndex = 0; blockIndex < blocksLength; blockIndex++)
			{
				var block = NonEmptyBlocks[blockIndex];
				var blockOffset = blockIndex << 6;
				while (block != 0UL)
				{
					var blockBit = (int)deBruijn[(int)(((block & (ulong)-(long)block) * 0x37E84A99DAE458FUL) >> 58)];

					var bitsIndex = blockOffset + blockBit;
					var bits = Bits[bitsIndex];
					var bitsOffset = bitsIndex << 6;
					while (bits != 0UL)
					{
						var bit = (int)deBruijn[(int)(((bits & (ulong)-(long)bits) * 0x37E84A99DAE458FUL) >> 58)];

						var id = bitsOffset + bit;
						BeforeDestroyed?.Invoke(id);
						RemoveBit(id);
						DestroyInWorld(id);

						EnsurePoolAt(PooledIds);
						Pool[PooledIds++] = id;
						MathUtils.IncrementWrapTo1(ref Versions[id]);

						bits &= bits - 1UL;
					}

					block &= block - 1UL;

					Bits[bitsIndex] = 0UL;
				}

				NonEmptyBlocks[blockIndex] = 0UL;
				SaturatedBlocks[blockIndex] = 0UL;
			}
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

		/// <remarks>
		/// Throws if the entity with this ID is not alive.
		/// </remarks>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Entity GetEntity(int id)
		{
			EntityNotAliveException.ThrowIfEntityDead(this, id);

			return new Entity(id, Versions[id], World);
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
			return id >= 0 && id < UsedIds && (Bits[id >> 6] & (1UL << (id & 63))) != 0UL;
		}

		/// <summary>
		/// Ensures the version array has sufficient capacity for the specified index.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void EnsureEntityAt(int index)
		{
			if (index >= VersionsCapacity)
			{
				Versions = Versions.ResizeToNextPowOf2(index + 1);
				if (Versions.Length > VersionsCapacity)
				{
					Array.Fill(Versions, 1U, VersionsCapacity, Versions.Length - VersionsCapacity);
				}
				VersionsCapacity = Versions.Length;
				Components.EnsureEntitiesCapacity(VersionsCapacity);
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void EnsurePoolAt(int index)
		{
			if (index >= Pool.Length)
			{
				Pool = Pool.ResizeToNextPowOf2(index + 1);
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public EntityEnumerator GetEnumerator()
		{
			var bitSet = QueryCache.Rent().AddToAll(this).Update();
			return new EntityEnumerator(bitSet, World);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void DestroyInWorld(int id)
		{
			var buffer = Components.Buffer;
			var componentCount = Components.GetAllAndRemove(id, buffer);

			for (var i = 0; i < componentCount; i++)
			{
				Sets.Lookup[buffer[i]].Remove(id);
			}

			Allocators.Free(id);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void SetBit(int id)
		{
			var bitsIndex = id >> 6;
			var blockIndex = id >> 12;

			EnsureBlocksCapacityAt(blockIndex);

			var bitsBit = 1UL << (id & 63);
			var blockBit = 1UL << (bitsIndex & 63);

			if (Bits[bitsIndex] == 0UL)
			{
				NonEmptyBlocks[blockIndex] |= blockBit;
			}
			Bits[bitsIndex] |= bitsBit;
			if (Bits[bitsIndex] == ulong.MaxValue)
			{
				SaturatedBlocks[blockIndex] |= blockBit;
			}

			for (var i = 0; i < RemoveOnAddCount; i++)
			{
				RemoveOnAdd[i].RemoveBit(bitsIndex, bitsBit);
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void RemoveBit(int id)
		{
			var bitsIndex = id >> 6;
			var blockIndex = id >> 12;

			if (bitsIndex >= Bits.Length)
			{
				return;
			}

			var bitsBit = 1UL << (id & 63);
			var blockBit = 1UL << (bitsIndex & 63);

			if (Bits[bitsIndex] == ulong.MaxValue)
			{
				SaturatedBlocks[blockIndex] &= ~blockBit;
			}
			Bits[bitsIndex] &= ~bitsBit;
			if (Bits[bitsIndex] == 0UL)
			{
				NonEmptyBlocks[blockIndex] &= ~blockBit;
			}

			for (var i = 0; i < RemoveOnRemoveCount; i++)
			{
				RemoveOnRemove[i].RemoveBit(bitsIndex, bitsBit);
			}
		}

		/// <summary>
		/// Creates and returns a new entities collection that is an exact copy of this one.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Entities Clone()
		{
			var clone = new Entities();
			CopyTo(clone);
			return clone;
		}

		/// <summary>
		/// Copies all entities and state from this entities collection into the specified one.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void CopyTo(Entities other)
		{
			CopyBitsTo(other);

			other.EnsureEntityAt(UsedIds - 1);
			other.EnsurePoolAt(PooledIds - 1);

			Array.Copy(Pool, other.Pool, PooledIds);
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
