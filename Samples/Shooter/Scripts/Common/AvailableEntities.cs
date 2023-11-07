using System;
using System.Collections.Generic;

namespace Massive.Samples.Shooter
{
    public class AvailableEntities<TState> where TState : struct
    {
        private readonly Stack<IEntity<TState>> _pool;
        private readonly IEntityFactory<TState> _factory;
        private readonly IEntity<TState>[] _entities;

        public AvailableEntities(IEntityFactory<TState> factory, int capacity)
        {
            _factory = factory;
            _entities = new IEntity<TState>[capacity];
            _pool = new Stack<IEntity<TState>>(capacity);
        }

        public int EntitiesCount { get; private set; }

        public Span<IEntity<TState>> Entities => new Span<IEntity<TState>>(_entities, 0, EntitiesCount);

        public void Reserve(int amount)
        {
            if (EntitiesCount + amount > _entities.Length)
            {
                throw new InvalidOperationException("Entity capacity exceeded!");
            }

            for (int i = 0; i < amount; i++)
            {
                var entity = ReserveEntity();
                entity.Enable();
                _entities[EntitiesCount] = entity;
                EntitiesCount += 1;
            }
        }

        public void Free(int amount)
        {
            if (EntitiesCount < amount)
            {
                throw new InvalidOperationException("Can't free this much! Not enough entities reserved.");
            }

            for (int i = EntitiesCount - amount; i < EntitiesCount; i++)
            {
                var entity = _entities[i];
                entity.Disable();
                _pool.Push(entity);
            }

            EntitiesCount -= amount;
        }

        private IEntity<TState> ReserveEntity()
        {
            if (_pool.Count != 0)
            {
                return _pool.Pop();
            }

            return _factory.Create();
        }
    }
}
