using System;
using System.Runtime.CompilerServices;
using Unity.IL2CPP.CompilerServices;

namespace Massive
{
	[Il2CppSetOption(Option.NullChecks | Option.ArrayBoundsChecks, false)]
	public class Entities : IdsSource
	{
		private int[] _ids;
		private uint[] _reuses;
		private int[] _sparse;

		public int MaxId { get; set; }

		public Entities()
		{
			_ids = Array.Empty<int>();
			_reuses = Array.Empty<uint>();
			_sparse = Array.Empty<int>();

			Ids = _ids;
		}

		public uint[] Reuses
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => _reuses;
		}

		public int[] Sparse
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => _sparse;
		}

		public ReadOnlySpan<int> Alive
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => new ReadOnlySpan<int>(Ids, 0, Count);
		}

		public event Action<int> AfterCreated;

		public event Action<int> BeforeDestroyed;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Entity Create()
		{
			EnsureCapacityForIndex(Count);

			Entity entity;

			// If there are unused elements in the packed array, return last
			if (Count < MaxId)
			{
				entity = GetEntityAt(Count);
			}
			else
			{
				entity = new Entity(MaxId, 0);
				AssignEntity(MaxId, 0, Count);
				MaxId += 1;
			}

			Count += 1;
			AfterCreated?.Invoke(entity.Id);
			return entity;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Destroy(int id)
		{
			// If ID is negative or element is not alive, nothing to be done
			if (id < 0 || id >= MaxId || Sparse[id] == Constants.InvalidId)
			{
				return;
			}

			BeforeDestroyed?.Invoke(id);

			Count -= 1;

			var index = Sparse[id];
			var reuseCount = Reuses[index];

			AssignEntity(Ids[Count], Reuses[Count], index);

			Sparse[id] = Constants.InvalidId;
			Ids[Count] = id;
			Reuses[Count] = unchecked(reuseCount + 1);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void CreateMany(int amount)
		{
			int needToCreate = amount;
			EnsureCapacityForIndex(needToCreate + Count);

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
			for (int i = Count - 1; i >= 0; i--)
			{
				var id = Ids[i];
				BeforeDestroyed?.Invoke(id);
				Count -= 1;
				Sparse[id] = Constants.InvalidId;
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

			return index != Constants.InvalidId && Reuses[index] == entity.ReuseCount;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool IsAlive(int id)
		{
			return id >= 0 && id < MaxId && Sparse[id] != Constants.InvalidId;
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
			int previousCapacity = _sparse.Length;
			Array.Resize(ref _sparse, capacity);
			if (capacity > previousCapacity)
			{
				Array.Fill(Sparse, Constants.InvalidId, previousCapacity, capacity - previousCapacity);
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
			return new Entity(Ids[index], Reuses[index]);
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
