using System;
using System.Collections.Generic;

namespace Massive.Samples.Shooter
{
	public class EntitySynchronisation<TState> where TState : struct
	{
		private readonly AvailableEntities<TState> _entities;

		public EntitySynchronisation(EntityFactory<TState> entityFactory)
		{
			_entities = new AvailableEntities<TState>(entityFactory);
		}

		public void Synchronize(in WorldFrame worldFrame, in Frame<TState> frame)
		{
			if (_entities.EntitiesCount < frame.AliveCount)
			{
				_entities.Reserve(frame.AliveCount - _entities.EntitiesCount);
			}
			
			if (_entities.EntitiesCount > frame.AliveCount)
			{
				_entities.Free(_entities.EntitiesCount - frame.AliveCount);
			}

			IReadOnlyList<EntityRoot<TState>> entities = _entities.Entities;
			Span<TState> states = frame.GetAll();
			for (int i = 0; i < states.Length; i++)
			{
				entities[i].SyncState(worldFrame, ref states[i]);
			}
		}
	}
}