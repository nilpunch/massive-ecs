using System;
using System.Runtime.CompilerServices;

namespace Massive
{
    [Unity.IL2CPP.CompilerServices.Il2CppSetOption (Unity.IL2CPP.CompilerServices.Option.NullChecks, false)]
    [Unity.IL2CPP.CompilerServices.Il2CppSetOption (Unity.IL2CPP.CompilerServices.Option.ArrayBoundsChecks, false)]
    [Unity.IL2CPP.CompilerServices.Il2CppSetOption (Unity.IL2CPP.CompilerServices.Option.DivideByZeroChecks, false)]
    public class WorldState<TState> : IWorldState where TState : struct, IState
    {
        private readonly int _framesCapacity;
        private readonly int _statesCapacity;
        private readonly TState[] _denseStatesByFrames;
        private readonly int[] _sparseByFrames;
        private readonly int[] _framesAliveCount;

        private int _currentFrame;
        private int _framesCount;

        public WorldState(int frames = 120, int statesCapacity = 100)
        {
            // Reserve 2 frames
            // One for rollback restoration, another one for current frame
            _framesCapacity = frames + 2;

            _statesCapacity = statesCapacity;
            _denseStatesByFrames = new TState[_framesCapacity * statesCapacity];
            _sparseByFrames = new int[_framesCapacity * statesCapacity];
            _framesAliveCount = new int[_framesCapacity];

            // Initialize sparse set
            for (int i = 0; i < _framesCapacity; i++)
            {
                Span<int> span = new Span<int>(_sparseByFrames, i * statesCapacity, statesCapacity);
                for (int j = 0; j < statesCapacity; j++)
                {
                    span[j] = j;
                }
            }
        }

        public int StatesCount => _framesAliveCount[_currentFrame];

        public int StatesCapacity => _statesCapacity;

        public bool CanReserveState => _framesAliveCount[_currentFrame] != _statesCapacity;

        public void SaveFrame()
        {
            int nextFrame = Loop(_currentFrame + 1, _framesCapacity);
            int currentAliveCount = _framesAliveCount[_currentFrame];

            if (currentAliveCount > 0)
            {
                int currentFrameIndex = _currentFrame * _statesCapacity;
                int nextFrameIndex = nextFrame * _statesCapacity;
                Array.Copy(_denseStatesByFrames, currentFrameIndex, _denseStatesByFrames, nextFrameIndex, currentAliveCount);
                Array.Copy(_sparseByFrames, currentFrameIndex, _sparseByFrames, nextFrameIndex, _statesCapacity);
            }

            _currentFrame = nextFrame;
            _framesAliveCount[nextFrame] = currentAliveCount;

            // Limit count by maxFrames-1, because one frame is current and so not counted
            _framesCount = Math.Min(_framesCount + 1, _framesCapacity - 1);
        }

        public void Rollback(int frames)
        {
            // One frame is reserved for restoring
            int canRollback = _framesCount - 1;

            if (frames > canRollback)
            {
                throw new InvalidOperationException($"Can't rollback this far. CanRollback:{canRollback}, Requested: {frames}.");
            }

            // Add one frame to the rollback to appear at one frame before the target frame
            frames += 1;

            _framesCount -= frames;
            _currentFrame = LoopNegative(_currentFrame - frames, _framesCapacity);

            // Populate target frame with data from rollback frame
            // This will keep rollback frame untouched
            SaveFrame();
        }

        public int Create(TState state = default)
        {
            int nextIndex = _framesAliveCount[_currentFrame];
            
            if (nextIndex == _statesCapacity)
            {
                throw new InvalidOperationException($"Exceeded limit of states per frame! Limit: {_statesCapacity}.");
            }

            state.SparseIndex = nextIndex;
            
            int denseIndex = _sparseByFrames[GlobalIndex(nextIndex)];
            _denseStatesByFrames[GlobalIndex(denseIndex)] = state;
            
            _framesAliveCount[_currentFrame] += 1;
            return nextIndex;
        }
        
        public void Delete(int sparseIndex)
        {
            int denseIndex = _sparseByFrames[GlobalIndex(sparseIndex)];
            int aliveCount = _framesAliveCount[_currentFrame];
            
            if (denseIndex >= aliveCount)
            {
                throw new InvalidOperationException($"Index is not alive! SparseIndex: {sparseIndex}.");
            }

            int swapDenseIndex = aliveCount - 1;
            TState swapState = _denseStatesByFrames[GlobalIndex(swapDenseIndex)];
            int swapSparseIndex = swapState.SparseIndex;
            
            _sparseByFrames[GlobalIndex(sparseIndex)] = swapDenseIndex;
            _sparseByFrames[GlobalIndex(swapSparseIndex)] = denseIndex;

            swapState.SparseIndex = sparseIndex;
            
            _denseStatesByFrames[GlobalIndex(denseIndex)] = swapState;
            
            _framesAliveCount[_currentFrame] -= 1;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ref TState Get(int sparseIndex)
        {
            if (!IsAlive(sparseIndex))
            {
                throw new InvalidOperationException($"State does not exist! RequestedState: {sparseIndex}.");
            }

            int denseId = _sparseByFrames[GlobalIndex(sparseIndex)];

            return ref _denseStatesByFrames[GlobalIndex(denseId)];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Span<TState> GetAll()
        {
            return new Span<TState>(_denseStatesByFrames, _currentFrame * _statesCapacity, _framesAliveCount[_currentFrame]);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsAlive(int sparseIndex)
        {
            return _sparseByFrames[GlobalIndex(sparseIndex)] < _framesAliveCount[_currentFrame];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private int GlobalIndex(int localIndex)
        {
            return _currentFrame * _statesCapacity + localIndex;
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
