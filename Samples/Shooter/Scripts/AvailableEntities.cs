using System;
using System.Collections.Generic;

namespace Massive.Samples.Shooter
{
	public class AvailableEntities<TState> where TState : struct
	{
		private readonly Stack<EntityRoot<TState>> _pool;
		private readonly EntityFactory<TState> _entityFactory;
		private readonly List<EntityRoot<TState>> _entities;

		public AvailableEntities(EntityFactory<TState> entityFactory)
		{
			_entityFactory = entityFactory;
			_entities = new List<EntityRoot<TState>>();
			_pool = new Stack<EntityRoot<TState>>();
		}

		public int EntitiesCount => _entities.Count;

		public IReadOnlyList<EntityRoot<TState>> Entities => _entities;

		public void Reserve(int amount)
		{
			for (int i = 0; i < amount; i++)
			{
				var entity = ReserveEntity();
				entity.Enable();
				_entities.Add(entity);
			}
		}

		public void Free(int amount)
		{
			if (EntitiesCount < amount)
			{
				throw new InvalidOperationException("Can't free this much! Not enough entities reserved.");
			}

			int finalCount = EntitiesCount - amount;
			for (int i = EntitiesCount - 1; i >= finalCount; i--)
			{
				var entity = _entities[i];
				entity.Disable();
				_pool.Push(entity);
				_entities.RemoveAt(i);
			}
		}

		private EntityRoot<TState> ReserveEntity()
		{
			if (_pool.Count != 0)
			{
				return _pool.Pop();
			}

			return _entityFactory.Create();
		}
	}
}