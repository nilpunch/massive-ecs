namespace Massive
{
    public readonly struct StateHandle<TState> where TState : struct
    {
        private readonly int _localIndex;
        private readonly WorldState<TState> _worldState;

        public StateHandle(int localIndex, WorldState<TState> worldState)
        {
            _localIndex = localIndex;
            _worldState = worldState;
        }

        public ref TState State => ref _worldState.Get(_localIndex);

        public bool IsExist => _worldState.IsExist(_localIndex);
    }
}
