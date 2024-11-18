using System;
using System.Runtime.CompilerServices;
using Unity.IL2CPP.CompilerServices;

namespace Massive
{
	[Il2CppSetOption(Option.NullChecks, false)]
	[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
	[Il2CppSetOption(Option.DivideByZeroChecks, false)]
	public class Entities : IdsSource
	{
		private const int EndHoleId = int.MaxValue;

		private int[] _ids = Array.Empty<int>();
		private uint[] _versions = Array.Empty<uint>();
		private int[] _sparse = Array.Empty<int>();

		public int MaxId { get; private set; }
		private int NextHoleId { get; set; } = EndHoleId;

		public Entities(Packing packing = Packing.Continuous)
		{
			Packing = packing;
			Ids = _ids;
		}

		/// <summary>
		/// Checks whether a packed array has no holes in it.
		/// </summary>
		public bool IsContinuous
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => Packing == Packing.Continuous || NextHoleId == EndHoleId;
		}

		/// <summary>
		/// Checks whether a packed array has any holes in it.
		/// </summary>
		public bool HasHoles
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => Packing == Packing.WithHoles && NextHoleId != EndHoleId;
		}

		public uint[] Versions => _versions;

		public int[] Sparse => _sparse;

		public event Action<int> AfterCreated;

		public event Action<int> BeforeDestroyed;

		/// <summary>
		/// For serialization and rollbacks only.
		/// </summary>
		public State CurrentState
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => new State(Count, MaxId, NextHoleId, Packing);
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			set
			{
				Count = value.Count;
				MaxId = value.MaxId;
				NextHoleId = value.NextHoleId;
				Packing = value.Packing;
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Entity Create()
		{
			EnsureCapacityAt(Count);

			Entity entity;

			if (Packing == Packing.WithHoles && NextHoleId != EndHoleId)
			{
				var id = NextHoleId;
				var index = Sparse[id];
				NextHoleId = ~Ids[index];
				Ids[index] = id;
				entity = Entity.Create(id, Versions[index]);
			}
			else if (Count < MaxId)
			{
				entity = GetEntityAt(Count);
				Count += 1;
			}
			else
			{
				entity = Entity.Create(MaxId, 0);
				AssignEntity(MaxId, 0, Count);
				MaxId += 1;
				Count += 1;
			}

			AfterCreated?.Invoke(entity.Id);
			return entity;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Destroy(int id)
		{
			// If ID is negative or element is not alive, nothing to be done
			if (id < 0 || id >= MaxId || Sparse[id] >= Count || Ids[Sparse[id]] != id)
			{
				return;
			}

			BeforeDestroyed?.Invoke(id);

			var index = Sparse[id];

			if (Packing == Packing.Continuous)
			{
				Count -= 1;
				var version = Versions[index];
				AssignEntity(Ids[Count], Versions[Count], index);
				AssignEntity(id, unchecked(version + 1), Count);
			}
			else
			{
				Ids[index] = ~NextHoleId;
				unchecked { Versions[index] += 1; }
				NextHoleId = id;
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void CreateMany(int amount)
		{
			var needToCreate = amount;
			EnsureCapacityAt(needToCreate + Count);

			while (Packing == Packing.WithHoles && NextHoleId != EndHoleId && needToCreate > 0)
			{
				needToCreate -= 1;
				var id = NextHoleId;
				var index = Sparse[id];
				NextHoleId = ~Ids[index];
				Ids[index] = id;
				AfterCreated?.Invoke(id);
			}

			while (Count < MaxId && needToCreate > 0)
			{
				needToCreate -= 1;
				Count += 1;
				AfterCreated?.Invoke(Ids[Count - 1]);
			}

			for (var i = 0; i < needToCreate; i++)
			{
				AssignEntity(MaxId, 0, Count);
				MaxId += 1;
				Count += 1;
				AfterCreated?.Invoke(MaxId - 1);
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Clear()
		{
			if (IsContinuous)
			{
				for (var i = Count - 1; i >= 0; i--)
				{
					var id = Ids[i];
					BeforeDestroyed?.Invoke(id);
					unchecked { Versions[i] += 1; }
					Count -= 1;
				}
			}
			else
			{
				for (var i = Count - 1; i >= 0; i--)
				{
					var id = Ids[i];
					if (id >= 0)
					{
						BeforeDestroyed?.Invoke(id);
						unchecked { Versions[i] += 1; }
					}
					Count -= 1;
				}

				var nextHoleId = NextHoleId;
				while (nextHoleId != EndHoleId)
				{
					var holeId = nextHoleId;
					var holeIndex = Sparse[holeId];
					nextHoleId = ~Ids[holeIndex];
					Ids[holeIndex] = holeId;
				}
				NextHoleId = EndHoleId;
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Entity GetEntity(int id)
		{
			return GetEntityAt(Sparse[id]);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool IsAlive(Entity entity)
		{
			if (entity.Id < 0 || entity.Id >= MaxId)
			{
				return false;
			}

			var index = Sparse[entity.Id];

			return index < Count && Ids[index] == entity.Id && Versions[index] == entity.Version;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool IsAlive(int id)
		{
			return id >= 0 && id < MaxId && Sparse[id] < Count && Ids[Sparse[id]] == id;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void EnsureCapacityAt(int index)
		{
			if (index >= Sparse.Length)
			{
				var newCapacity = MathUtils.NextPowerOf2(index + 1);
				ResizePacked(newCapacity);
				ResizeSparse(newCapacity);
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void ResizePacked(int capacity)
		{
			Array.Resize(ref _ids, capacity);
			Array.Resize(ref _versions, capacity);
			Ids = _ids;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void ResizeSparse(int capacity)
		{
			Array.Resize(ref _sparse, capacity);
		}

		public override Packing ExchangePacking(Packing packing)
		{
			var previousPacking = Packing;
			if (packing != Packing)
			{
				if (packing == Packing.Continuous)
				{
					Compact();
				}
				Packing = packing;
			}
			return previousPacking;
		}

		/// <summary>
		/// Removes all holes from the ids.
		/// </summary>
		public void Compact()
		{
			if (HasHoles)
			{
				var count = Count;
				var nextHoleId = NextHoleId;

				for (; count > 0 && Ids[count - 1] < 0; count--) { }

				while (nextHoleId != EndHoleId)
				{
					var holeId = nextHoleId;
					var holeIndex = Sparse[holeId];
					nextHoleId = ~Ids[holeIndex];
					if (holeIndex < count)
					{
						count -= 1;

						var holeVersion = Versions[holeIndex];
						AssignEntity(Ids[count], Versions[count], holeIndex);
						AssignEntity(holeId, holeVersion, count);

						for (; count > 0 && Ids[count - 1] < 0; count--) { }
					}
					else
					{
						Ids[holeIndex] = holeId;
					}
				}

				Count = count;
				NextHoleId = EndHoleId;
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public IdsEnumerator GetEnumerator()
		{
			return new IdsEnumerator(this);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private Entity GetEntityAt(int index)
		{
			return Entity.Create(Ids[index], Versions[index]);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void AssignEntity(int id, uint version, int index)
		{
			Sparse[id] = index;
			Ids[index] = id;
			Versions[index] = version;
		}

		public readonly struct State
		{
			public readonly int Count;
			public readonly int MaxId;
			public readonly int NextHoleId;
			public readonly Packing Packing;

			public State(int count, int maxId, int nextHoleId, Packing packing)
			{
				Count = count;
				MaxId = maxId;
				NextHoleId = nextHoleId;
				Packing = packing;
			}
		}
	}
}
