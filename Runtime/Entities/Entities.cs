using System;
using System.Diagnostics.CodeAnalysis;
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

		public event Action<Entity> AfterCreated;

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
				MaxId += 1;
				AssignEntity(entity, Count);
			}

			Count += 1;
			AfterCreated?.Invoke(entity);
			return entity;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Destroy(int id)
		{
			// If ID is negative or element is not alive, nothing to be done
			if (id < 0 || id >= MaxId)
			{
				return;
			}
			var index = Sparse[id];
			var entity = GetEntityAt(index);
			if (index >= Count || entity.Id != id)
			{
				return;
			}

			BeforeDestroyed?.Invoke(id);

			Count -= 1;

			// Swap packed with last element
			AssignEntity(GetEntityAt(Count), index);
			AssignEntity(Entity.Reuse(entity), Count);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void CreateMany(int amount, [MaybeNull] Action<Entity> action = null)
		{
			int needToCreate = amount;
			EnsureCapacityForIndex(needToCreate + Count);

			while (Count < MaxId && needToCreate > 0)
			{
				int count = Count;
				Count += 1;
				needToCreate -= 1;
				action?.Invoke(GetEntityAt(count));
			}

			for (int i = 0; i < needToCreate; i++)
			{
				var newEntity = new Entity(MaxId, 0);
				AssignEntity(newEntity, Count);
				Count += 1;
				MaxId += 1;
				action?.Invoke(newEntity);
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
			if (id < 0 || id >= MaxId)
			{
				return false;
			}

			int index = Sparse[id];

			return index < Count && Ids[index] == id;
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

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private Entity GetEntityAt(int index)
		{
			return new Entity(Ids[index], Reuses[index]);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void AssignEntity(Entity entity, int index)
		{
			Sparse[entity.Id] = index;
			Ids[index] = entity.Id;
			Reuses[index] = entity.ReuseCount;
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
	}
}
