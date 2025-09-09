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
	public class Entifiers : BitsBase, IBitsBacktrack
	{
		private Bits[] RemoveOnAdd { get; set; } = Array.Empty<Bits>();
		private int RemoveOnAddCount { get; set; }

		private Bits[] RemoveOnRemove { get; set; } = Array.Empty<Bits>();
		private int RemoveOnRemoveCount { get; set; }

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
			if (id >= UsedIds || (Bits0[id >> 6] & (1UL << (id & 63))) == 0UL)
			{
				return false;
			}

			BeforeDestroyed?.Invoke(id);
			RemoveBit(id);
			WorldContext?.EntityDestroyed(id);

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
			var bits1Length = Bits1.Length;

			for (var current1 = 0; current1 < bits1Length; current1++)
			{
				var offset1 = current1 << 6;
				var iterated1 = 0;

				while (Bits1[current1] != 0UL && iterated1 < 64)
				{
					var bits1Result = Bits1[current1] >> iterated1;

					var skip1 = MathUtils.LSB(bits1Result);
					iterated1 += skip1;
					bits1Result >>= skip1;

					if (bits1Result == 0UL)
					{
						break;
					}

					var runLength1 = bits1Result == ulong.MaxValue ? 64 : MathUtils.LSB(~bits1Result);
					var runEnd1 = iterated1 + runLength1;
					for (; iterated1 < runEnd1; iterated1++)
					{
						if ((Bits1[current1] & (1UL << iterated1)) == 0)
						{
							continue;
						}

						var current0 = offset1 + iterated1;

						var offset0 = current0 << 6;
						var iterated0 = 0;

						while (Bits0[current0] != 0UL && iterated0 < 64)
						{
							var bits0Result = Bits0[current0] >> iterated0;

							var skip0 = MathUtils.LSB(bits0Result);
							iterated0 += skip0;
							bits0Result >>= skip0;

							if (bits0Result == 0UL)
							{
								break;
							}

							var runLength0 = bits0Result == ulong.MaxValue ? 64 : MathUtils.LSB(~bits0Result);
							var runEnd0 = iterated0 + runLength0;
							for (; iterated0 < runEnd0; iterated0++)
							{
								if ((Bits0[current0] & (1UL << iterated0)) == 0)
								{
									continue;
								}

								var id = offset0 + iterated0;
								BeforeDestroyed?.Invoke(id);
								RemoveBit(id);
								WorldContext?.EntityDestroyed(id);

								EnsurePoolAt(PooledIds);
								Pool[PooledIds++] = id;
								MathUtils.IncrementWrapTo1(ref Versions[id]);
							}
						}
					}
				}
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
				WorldContext?.Components.EnsureEntitiesCapacity(VersionsCapacity);
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
		public BitsEnumerator GetEnumerator()
		{
			var bits = BitsPool.RentClone(this).RemoveOnRemove(this);
			return new BitsEnumerator(bits, Bits1.Length);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void SetBit(int id)
		{
			var id0 = id >> 6;
			var id1 = id >> 12;

			if (id1 >= Bits1.Length)
			{
				Bits1 = Bits1.Resize(MathUtils.NextPowerOf2(id1 + 1));
				Bits0 = Bits0.Resize(Bits1.Length << 6);
			}

			var bit0 = 1UL << (id & 63);
			var bit1 = 1UL << (id0 & 63);

			if (Bits0[id0] == 0UL)
			{
				Bits1[id1] |= bit1;
			}
			Bits0[id0] |= bit0;

			for (var i = 0; i < RemoveOnAddCount; i++)
			{
				RemoveOnAdd[i].Remove(id);
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void RemoveBit(int id)
		{
			var id0 = id >> 6;
			var id1 = id >> 12;

			if (id0 >= Bits0.Length)
			{
				return;
			}

			var bit0 = 1UL << (id & 63);
			var bit1 = 1UL << (id0 & 63);

			Bits0[id0] &= ~bit0;
			if (Bits0[id0] == 0UL)
			{
				Bits1[id1] &= ~bit1;
			}

			for (var i = 0; i < RemoveOnRemoveCount; i++)
			{
				RemoveOnRemove[i].Remove(id);
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		void IBitsBacktrack.PushRemoveOnAdd(Bits bits)
		{
			if (RemoveOnAddCount >= RemoveOnAdd.Length)
			{
				RemoveOnAdd = RemoveOnAdd.ResizeToNextPowOf2(RemoveOnAddCount + 1);
			}

			RemoveOnAdd[RemoveOnAddCount++] = bits;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		void IBitsBacktrack.PopRemoveOnAdd()
		{
			RemoveOnAddCount--;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		void IBitsBacktrack.PushRemoveOnRemove(Bits bits)
		{
			if (RemoveOnRemoveCount >= RemoveOnRemove.Length)
			{
				RemoveOnRemove = RemoveOnRemove.ResizeToNextPowOf2(RemoveOnRemoveCount + 1);
			}

			RemoveOnRemove[RemoveOnRemoveCount++] = bits;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		void IBitsBacktrack.PopRemoveOnRemove()
		{
			RemoveOnRemoveCount--;
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
