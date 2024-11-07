﻿using System;
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

		private int[] _ids;
		private uint[] _reuses;
		private int[] _sparse;

		public int MaxId { get; set; }
		public int NextHoleId { get; set; }

		public Entities(PackingMode packingMode = PackingMode.Continuous)
		{
			_ids = Array.Empty<int>();
			_reuses = Array.Empty<uint>();
			_sparse = Array.Empty<int>();
			PackingMode = packingMode;

			Ids = _ids;
			NextHoleId = EndHoleId;
		}

		/// <summary>
		/// Checks whether a packed array has no holes in it.
		/// </summary>
		public bool IsContinuous
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => PackingMode == PackingMode.Continuous || NextHoleId == EndHoleId;
		}

		/// <summary>
		/// Checks whether a packed array has any holes in it.
		/// </summary>
		public bool HasHoles
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => PackingMode == PackingMode.WithHoles && NextHoleId != EndHoleId;
		}

		public uint[] Reuses => _reuses;

		public int[] Sparse => _sparse;

		public event Action<int> AfterCreated;

		public event Action<int> BeforeDestroyed;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Entity Create()
		{
			EnsureCapacityForIndex(Count);

			Entity entity;

			if (HasHoles)
			{
				int id = NextHoleId;
				int index = Sparse[id];
				NextHoleId = ~Ids[index];
				Ids[index] = id;
				entity = Entity.Create(id, Reuses[index]);
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

			if (PackingMode == PackingMode.Continuous)
			{
				Count -= 1;

				var index = Sparse[id];
				var reuseCount = Reuses[index];

				AssignEntity(Ids[Count], Reuses[Count], index);
				AssignEntity(id, unchecked(reuseCount + 1), Count);
			}
			else
			{
				int index = Sparse[id];
				Ids[index] = ~NextHoleId;
				unchecked { Reuses[index] += 1; }
				NextHoleId = id;
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void CreateMany(int amount)
		{
			int needToCreate = amount;
			EnsureCapacityForIndex(needToCreate + Count);

			while (HasHoles && needToCreate > 0)
			{
				needToCreate -= 1;
				int id = NextHoleId;
				int index = Sparse[id];
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

			for (int i = 0; i < needToCreate; i++)
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
			Compact();

			for (int i = Count - 1; i >= 0; i--)
			{
				var id = Ids[i];
				BeforeDestroyed?.Invoke(id);
				Count -= 1;
				unchecked { Reuses[i] += 1; }
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

			int index = Sparse[entity.Id];

			return index < Count && Ids[index] == entity.Id && Reuses[index] == entity.ReuseCount;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool IsAlive(int id)
		{
			return id >= 0 && id < MaxId && Sparse[id] < Count && Ids[Sparse[id]] == id;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void EnsureCapacityForIndex(int index)
		{
			if (index >= Sparse.Length)
			{
				int newCapacity = MathHelpers.NextPowerOf2(index + 1);
				ResizePacked(newCapacity);
				ResizeSparse(newCapacity);
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void ResizePacked(int capacity)
		{
			Array.Resize(ref _ids, capacity);
			Array.Resize(ref _reuses, capacity);
			Ids = _ids;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void ResizeSparse(int capacity)
		{
			Array.Resize(ref _sparse, capacity);
		}

		public override void ChangePackingMode(PackingMode value)
		{
			if (value != PackingMode)
			{
				PackingMode = value;
				Compact();
			}
		}

		/// <summary>
		/// Removes all holes from the ids.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Compact()
		{
			if (HasHoles)
			{
				for (; Count > 0 && Ids[Count - 1] < 0; Count--) { }

				while (NextHoleId != EndHoleId)
				{
					int holeId = NextHoleId;
					int holeIndex = Sparse[NextHoleId];
					NextHoleId = ~Ids[NextHoleId];
					if (holeIndex < Count)
					{
						Count -= 1;

						var holeReuseCount = Reuses[holeIndex];
						AssignEntity(Ids[Count], Reuses[Count], holeIndex);
						AssignEntity(holeId, holeReuseCount, Count);

						for (; Count > 0 && Ids[Count - 1] < 0; Count--) { }
					}
				}
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public IdsSourceEnumerator GetEnumerator()
		{
			return new IdsSourceEnumerator(this);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private Entity GetEntityAt(int index)
		{
			return Entity.Create(Ids[index], Reuses[index]);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void AssignEntity(int id, uint reuseCount, int index)
		{
			Sparse[id] = index;
			Ids[index] = id;
			Reuses[index] = reuseCount;
		}
	}
}
