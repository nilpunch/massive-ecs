using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using Unity.IL2CPP.CompilerServices;

namespace Massive
{
	[Il2CppSetOption(Option.NullChecks, false)]
	[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
	public class Entities
	{
		private Entity[] _dense;
		private int[] _sparse;

		protected Entity[] Dense => _dense;
		protected int[] Sparse => _sparse;

		protected int AliveCount { get; set; }
		protected int MaxId { get; set; }

		public Entities(int dataCapacity = Constants.DataCapacity)
		{
			_dense = new Entity[dataCapacity];
			_sparse = new int[dataCapacity];
		}

		public int CanCreateAmount => Dense.Length - AliveCount;

		public ReadOnlySpan<Entity> Alive => new ReadOnlySpan<Entity>(Dense, 0, AliveCount);

		public event Action<Entity> AfterCreated;

		public event Action<int> BeforeDeleted;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Entity Create()
		{
			int count = AliveCount;
			AliveCount += 1;

			if (count == Dense.Length)
			{
				GrowCapacity(count + 1);
			}

			Entity entity;
			int maxId = MaxId;

			// If there are unused elements in the dense array, return last
			if (count < maxId)
			{
				entity = Dense[count];
			}
			else
			{
				entity = new Entity(maxId, 0);
				MaxId += 1;
				AssignEntity(entity, count);
			}

			AfterCreated?.Invoke(entity);
			return entity;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Delete(Entity entity)
		{
			BeforeDeleted?.Invoke(entity.Id);

			// If element is not alive, nothing to be done
			if (entity.Id < 0 || entity.Id >= MaxId)
			{
				return;
			}
			var dense = Sparse[entity.Id];
			if (dense >= AliveCount || Dense[dense] != entity)
			{
				return;
			}

			int count = AliveCount;
			AliveCount -= 1;

			// If dense is the last used element, decreasing alive count and apply reuse is enough
			if (dense == count - 1)
			{
				Dense[dense] = Entity.Reuse(entity);
				return;
			}

			// Swap dense with last element
			int lastDense = count - 1;
			AssignEntity(Dense[lastDense], dense);
			AssignEntity(Entity.Reuse(entity), lastDense);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Delete(int id)
		{
			BeforeDeleted?.Invoke(id);

			// If element is not alive, nothing to be done
			if (id < 0 || id >= MaxId)
			{
				return;
			}
			var dense = Sparse[id];
			if (dense >= AliveCount)
			{
				return;
			}
			var entity = Dense[dense];
			if (entity.Id != id)
			{
				return;
			}

			int count = AliveCount;
			AliveCount -= 1;

			// If dense is the last used element, decreasing alive count and apply reuse is enough
			if (dense == count - 1)
			{
				Dense[dense] = Entity.Reuse(entity);
				return;
			}

			// Swap dense with last element
			int lastDense = count - 1;
			AssignEntity(Dense[lastDense], dense);
			AssignEntity(Entity.Reuse(entity), lastDense);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void CreateMany(int amount, [MaybeNull] Action<Entity> action = null)
		{
			int needToCreate = amount;
			if (needToCreate + AliveCount >= Dense.Length)
			{
				GrowCapacity(needToCreate + AliveCount + 1);
			}

			while (AliveCount < MaxId && needToCreate > 0)
			{
				int count = AliveCount;
				AliveCount += 1;
				action?.Invoke(Dense[count]);
				needToCreate -= 1;
			}

			for (int i = 0; i < needToCreate; i++)
			{
				int count = AliveCount;
				int maxId = MaxId;
				var newId = new Entity(maxId, 0);
				AliveCount += 1;
				MaxId += 1;
				AssignEntity(newId, count);
				action?.Invoke(newId);
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Entity GetEntity(int sparseId)
		{
			return Dense[sparseId];
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool IsAlive(Entity entity)
		{
			if (entity.Id < 0 || entity.Id >= MaxId)
			{
				return false;
			}

			int dense = Sparse[entity.Id];

			return dense < AliveCount && Dense[dense] == entity;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool IsAlive(int id)
		{
			if (id < 0 || id >= MaxId)
			{
				return false;
			}

			int dense = Sparse[id];

			return dense < AliveCount && Dense[dense].Id == id;
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
			int newCapacity = MathHelpers.GetNextPowerOf2(desiredCapacity);
			ResizeDense(newCapacity);
			ResizeSparse(newCapacity);
		}
	}
}