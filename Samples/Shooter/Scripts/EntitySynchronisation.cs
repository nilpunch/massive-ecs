using System;
using System.Collections.Generic;

namespace MassiveData.Samples.Shooter
{
	public class EntitySynchronisation<TState> where TState : struct
	{
		private readonly AvailableEntities<TState> _entities;

		public EntitySynchronisation(EntityFactory<TState> entityFactory)
		{
			_entities = new AvailableEntities<TState>(entityFactory);
		}

		public void Synchronize(Massive<TState> data)
		{
			var aliveCount = data.AliveCount;

			if (_entities.EntitiesCount < aliveCount)
			{
				_entities.Reserve(aliveCount - _entities.EntitiesCount);
			}
			else if (_entities.EntitiesCount > aliveCount)
			{
				_entities.Free(_entities.EntitiesCount - aliveCount);
			}

			IReadOnlyList<EntityRoot<TState>> entities = _entities.Entities;
			Span<TState> states = data.AliveData;
			for (int i = 0; i < aliveCount; i++)
			{
				entities[i].SyncState(ref states[i]);
			}
		}
	}
}