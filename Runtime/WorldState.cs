using System;
using System.Runtime.CompilerServices;

namespace Massive
{
#if ENABLE_IL2CPP
    [Unity.IL2CPP.CompilerServices.Il2CppSetOption (Unity.IL2CPP.CompilerServices.Option.NullChecks, false)]
    [Unity.IL2CPP.CompilerServices.Il2CppSetOption (Unity.IL2CPP.CompilerServices.Option.ArrayBoundsChecks, false)]
#endif
    public class WorldState<TState> where TState : struct
    {
        private readonly int _framesCapacity;
        private readonly int _statesPerFrame;
        private readonly TState[] _statesByFrames;
        private readonly int[] _framesLength;
        private int _currentFrame;
        private int _framesCount;

        public WorldState(int frames = 120, int statesPerFrame = 100)
        {
            // Reserve 2 frames. One for rollback restoration, other one for current frame.
            _framesCapacity = frames + 2;

            _statesPerFrame = statesPerFrame;
            _statesByFrames = new TState[_framesCapacity * statesPerFrame];
            _framesLength = new int[_framesCapacity];
        }

        public void SaveFrame()
        {
            int nextFrame = Loop(_currentFrame + 1, _framesCapacity);
            int currentFrameLength = _framesLength[_currentFrame];

            if (currentFrameLength > 0)
            {
                int currentFrameIndex = _currentFrame * _statesPerFrame;
                int nextFrameIndex = nextFrame * _statesPerFrame;
                Array.Copy(_statesByFrames, currentFrameIndex, _statesByFrames, nextFrameIndex, currentFrameLength);
            }

            _currentFrame = nextFrame;
            _framesLength[nextFrame] = currentFrameLength;

            // Limit count by maxFrames-1, because one frame is current and so not counted.
            _framesCount = Math.Min(_framesCount + 1, _framesCapacity - 1);
        }

        public void Rollback(int rollbackFrames)
        {
            // One frame is reserved for restoring.
            int canRollback = _framesCount - 1;

            if (rollbackFrames > canRollback)
            {
                throw new InvalidOperationException($"Can't rollback this far. CanRollback:{canRollback}, Requested: {rollbackFrames}.");
            }

            // Add one frame to the rollback to appear at one frame before the target frame.
            rollbackFrames += 1;

            _framesCount -= rollbackFrames;
            _currentFrame = LoopNegative(_currentFrame - rollbackFrames, _framesCapacity);

            // Populate target frame with data from rollback frame.
            // This will keep rollback frame untouched.
            SaveFrame();
        }

        public StateHandle<TState> Reserve(TState state)
        {
            if (_framesLength[_currentFrame] == _statesPerFrame)
            {
                throw new InvalidOperationException($"Exceeded limit of states per frame! Limit: {_statesPerFrame}.");
            }

            int localIndex = _framesLength[_currentFrame];
            _statesByFrames[GlobalIndex(localIndex)] = state;
            _framesLength[_currentFrame] += 1;
            return new StateHandle<TState>(localIndex, this);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ref TState Get(int localIndex)
        {
            if (!IsExist(localIndex))
            {
                throw new InvalidOperationException($"State does not exist! RequestedState: {localIndex}.");
            }

            return ref _statesByFrames[GlobalIndex(localIndex)];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Span<TState> GetAll()
        {
            return new Span<TState>(_statesByFrames, _currentFrame * _statesPerFrame, _framesLength[_currentFrame]);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsExist(int localIndex)
        {
            return localIndex < _framesLength[_currentFrame];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private int GlobalIndex(int localIndex)
        {
            return _currentFrame * _statesPerFrame + localIndex;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int Loop(int a, int b)
		{
            return a % b;
		}

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int LoopNegative(int a, int b)
        {
            int result = a % b;

            if (result < 0)
            {
                return result + b;
            }

            return result;
        }
    }
}
