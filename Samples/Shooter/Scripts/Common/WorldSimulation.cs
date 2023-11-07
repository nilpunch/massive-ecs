using System;

namespace Massive.Samples.Shooter
{
    public class WorldSimulation<TState> : ISimulation, IWorldState where TState : struct
    {
        private readonly WorldState<TState> _worldState;
        private readonly AvailableEntities<TState> _entities;

        public WorldSimulation(WorldState<TState> worldState, AvailableEntities<TState> entities)
        {
            _worldState = worldState;
            _entities = entities;
        }

        public void StepForward()
        {
            if (_entities.EntitiesCount < _worldState.StatesCount)
            {
                _entities.Reserve(_worldState.StatesCount - _entities.EntitiesCount);
            }
            if (_entities.EntitiesCount > _worldState.StatesCount)
            {
                _entities.Free(_entities.EntitiesCount - _worldState.StatesCount);
            }

            Span<IEntity<TState>> entities = _entities.Entities;
            Span<TState> states = _worldState.GetAll();
            for (int i = 0; i < states.Length; i++)
            {
                entities[i].UpdateState(ref states[i]);
            }
        }

        public void SaveFrame()
        {
            _worldState.SaveFrame();
        }

        public void Rollback(int frames)
        {
            _worldState.Rollback(frames);
        }
    }
}
