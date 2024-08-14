using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using Unity.IL2CPP.CompilerServices;

namespace Massive
{
	[Il2CppSetOption(Option.NullChecks | Option.ArrayBoundsChecks, false)]
	public class Entities
	{
		private Entity[] _dense;
		private int[] _sparse;

		public int Count { get; set; }

		public int MaxId { get; set; }

		public Entities(int setCapacity = Constants.DefaultSetCapacity)
		{
			_dense = new Entity[setCapacity];
			_sparse = new int[setCapacity];
		}

		public Entity[] Dense
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => _dense;
		}

		public int[] Sparse
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => _sparse;
		}

		public ReadOnlySpan<Entity> Alive
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => new ReadOnlySpan<Entity>(Dense, 0, Count);
		}

		public event Action<Entity> AfterCreated;

		public event Action<int> BeforeDestroyed;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Entity Create()
		{
			if (Count == Dense.Length)
			{
				GrowCapacity(Count + 1);
			}

			Entity entity;

			// If there are unused elements in the dense array, return last
			if (Count < MaxId)
			{
				entity = Dense[Count];
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
			var dense = Sparse[id];
			var entity = Dense[dense];
			if (dense >= Count || entity.Id != id)
			{
				return;
			}

			BeforeDestroyed?.Invoke(id);

			Count -= 1;

			// Swap dense with last element
			AssignEntity(Dense[Count], dense);
			AssignEntity(Entity.Reuse(entity), Count);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void CreateMany(int amount, [MaybeNull] Action<Entity> action = null)
		{
			int needToCreate = amount;
			if (needToCreate + Count >= Dense.Length)
			{
				GrowCapacity(needToCreate + Count + 1);
			}

			while (Count < MaxId && needToCreate > 0)
			{
				int count = Count;
				Count += 1;
				needToCreate -= 1;
				action?.Invoke(Dense[count]);
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
			return Dense[Sparse[id]];
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool IsAlive(Entity entity)
		{
			if (entity.Id < 0 || entity.Id >= MaxId)
			{
				return false;
			}

			int dense = Sparse[entity.Id];

			return dense < Count && Dense[dense] == entity;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool IsAlive(int id)
		{
			if (id < 0 || id >= MaxId)
			{
				return false;
			}

			int dense = Sparse[id];

			return dense < Count && Dense[dense].Id == id;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public virtual void ResizeDense(int capacity)
		{
			Array.Resize(ref _dense, capacity);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public virtual void ResizeSparse(int capacity)
		{
			Array.Resize(ref _sparse, capacity);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void AssignEntity(Entity entity, int dense)
		{
			Sparse[entity.Id] = dense;
			Dense[dense] = entity;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void GrowCapacity(int desiredCapacity)
		{
			int newCapacity = MathHelpers.NextPowerOf2(desiredCapacity);
			ResizeDense(newCapacity);
			ResizeSparse(newCapacity);
		}
	}
}
