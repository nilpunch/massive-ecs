using System;
using System.Runtime.CompilerServices;
using Unity.IL2CPP.CompilerServices;

namespace Massive
{
	[Il2CppSetOption(Option.NullChecks, false)]
	[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
	[Il2CppSetOption(Option.DivideByZeroChecks, false)]
	public class Entities : PackedSet
	{
		private const int EndHoleId = int.MaxValue;

		public uint[] Versions { get; private set; } = Array.Empty<uint>();
		public int[] Sparse { get; private set; } = Array.Empty<int>();

		public int PackedCapacity { get; private set; }
		public int SparseCapacity { get; private set; }

		public int MaxId { get; private set; }
		private int NextHoleId { get; set; } = EndHoleId;

		public Entities(Packing packing = Packing.Continuous)
		{
			Packing = packing;
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
			get => Packing != Packing.Continuous && NextHoleId != EndHoleId;
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
				NextHoleId = ~Packed[index];
				Packed[index] = id;
				entity = new Entity(id, Versions[index]);
			}
			else if (Count < MaxId)
			{
				entity = GetEntityAt(Count);
				Count += 1;
			}
			else
			{
				entity = new Entity(MaxId, 0);
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
			// If ID is negative or entity is not alive, nothing to be done.
			if (id < 0 || id >= MaxId || Sparse[id] >= Count || Packed[Sparse[id]] != id)
			{
				return;
			}

			BeforeDestroyed?.Invoke(id);

			var index = Sparse[id];

			if (Packing == Packing.Continuous)
			{
				Count -= 1;
				var version = Versions[index];
				AssignEntity(Packed[Count], Versions[Count], index);
				AssignEntity(id, unchecked(version + 1), Count);
			}
			else
			{
				Packed[index] = ~NextHoleId;
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
				NextHoleId = ~Packed[index];
				Packed[index] = id;
				AfterCreated?.Invoke(id);
			}

			while (Count < MaxId && needToCreate > 0)
			{
				needToCreate -= 1;
				Count += 1;
				AfterCreated?.Invoke(Packed[Count - 1]);
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
					var id = Packed[i];
					BeforeDestroyed?.Invoke(id);
					unchecked { Versions[i] += 1; }
					Count -= 1;
				}
			}
			else
			{
				for (var i = Count - 1; i >= 0; i--)
				{
					var id = Packed[i];
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
					nextHoleId = ~Packed[holeIndex];
					Packed[holeIndex] = holeId;
				}
				NextHoleId = EndHoleId;
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Entity GetEntity(int id)
		{
			return new Entity(id, Versions[Sparse[id]]);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool IsAlive(Entity entity)
		{
			if (entity.Id < 0 || entity.Id >= MaxId)
			{
				return false;
			}

			var index = Sparse[entity.Id];

			return index < Count && Packed[index] == entity.Id && Versions[index] == entity.Version;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool IsAlive(int id)
		{
			return id >= 0 && id < MaxId && Sparse[id] < Count && Packed[Sparse[id]] == id;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void EnsureCapacityAt(int index)
		{
			if (index >= SparseCapacity)
			{
				var newCapacity = MathUtils.NextPowerOf2(index + 1);
				ResizePacked(newCapacity);
				ResizeSparse(newCapacity);
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void ResizePacked(int capacity)
		{
			Packed = Packed.Resize(capacity);
			Versions = Versions.Resize(capacity);
			PackedCapacity = capacity;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void ResizeSparse(int capacity)
		{
			Sparse = Sparse.Resize(capacity);
			SparseCapacity = capacity;
		}

		public override void Compact()
		{
			if (HasHoles)
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

						var holeVersion = Versions[holeIndex];
						AssignEntity(Packed[count], Versions[count], holeIndex);
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
		public PackedEnumerator GetEnumerator()
		{
			return new PackedEnumerator(this);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private Entity GetEntityAt(int index)
		{
			return new Entity(Packed[index], Versions[index]);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void AssignEntity(int id, uint version, int index)
		{
			Sparse[id] = index;
			Packed[index] = id;
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
