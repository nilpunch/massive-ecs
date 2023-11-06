using System;

namespace Massive
{
    public class WorldState<TState> where TState : struct
    {
        private readonly int _maxFrames;
        private readonly int _maxStatesPerFrame;
        private readonly TState[] _continuousState;
        private readonly int[] _frameLengths;
        private readonly int[] _frameStarts;

        private int _currentFrame;
        private int _savedFrames;

        public WorldState(int maxFrames = 120, int maxStatesPerFrame = 100)
        {
            // Reserve 1 frame for rollback state restoration.
            _maxFrames = maxFrames + 1;
            _maxStatesPerFrame = maxStatesPerFrame;
            _continuousState = new TState[maxStatesPerFrame * _maxFrames];
            _frameLengths = new int[_maxFrames];
            _frameStarts = new int[_maxFrames];

            _frameLengths[0] = 0;
            _frameStarts[0] = 0;
        }

        public void SaveFrame()
        {
            int currentLength = _frameLengths[_currentFrame];
            int currentStartIndex = _frameStarts[_currentFrame];

            int nextStartIndex = currentStartIndex + currentLength;
            int nextEndIndex = nextStartIndex + currentLength;

            if (currentLength > 0)
            {
                // 3______12 -> _123_____
                if (nextStartIndex >= _continuousState.Length)
                {
                    int residualLength = nextStartIndex - _continuousState.Length;
                    int copyLength = _continuousState.Length - currentStartIndex;

                    // First half:
                    // 3______12 -> 312______
                    Array.Copy(_continuousState, currentStartIndex, _continuousState, residualLength, copyLength);

                    if (residualLength > 0)
                    {
                        // Second half:
                        // 312______ -> _123_____
                        Array.Copy(_continuousState, 0, _continuousState, residualLength + copyLength, residualLength);
                    }
                }
                // _____123_ -> 23______1
                else if (nextEndIndex > _continuousState.Length)
                {
                    int copyLength = _continuousState.Length - nextStartIndex;

                    // First half:
                    // _____123_ -> ______231
                    Array.Copy(_continuousState, currentStartIndex, _continuousState, nextStartIndex, copyLength);

                    int residualLength = nextEndIndex - _continuousState.Length;
                    if (residualLength > 0)
                    {
                        // Second half:
                        // ______231 -> 23______1
                        Array.Copy(_continuousState, currentStartIndex + copyLength, _continuousState, 0, residualLength);
                    }
                }
                // ___123___ -> ______123
                else
                {
                    Array.Copy(_continuousState, currentStartIndex, _continuousState, nextStartIndex, currentLength);
                }
            }

            _savedFrames = Math.Min(_savedFrames + 1, _maxFrames);

            int nextFrame = Loop(_currentFrame + 1, _maxFrames);
            _currentFrame = nextFrame;
            _frameStarts[nextFrame] = Loop(nextStartIndex, _continuousState.Length);
            _frameLengths[nextFrame] = currentLength;
        }

        public void Rollback(int rollbackFrames)
        {
            // One frame is reserved for data restoration.
            int canRollback = _savedFrames - 1;

            if (rollbackFrames > canRollback)
            {
                throw new InvalidOperationException($"Can't rollback this far. CanRollback:{canRollback}, Requested: {rollbackFrames}");
            }

            // Add one frame to the rollback to appear at one frame before the target frame.
            rollbackFrames += 1;

            _savedFrames -= rollbackFrames;
            _currentFrame = LoopNegative(_currentFrame - rollbackFrames, _maxFrames);

            // Populate target frame with data from previous frame.
            SaveFrame();
        }

        public StateHandle<TState> Reserve(TState state)
        {
            if (_frameLengths[_currentFrame] == _maxStatesPerFrame)
            {
                throw new InvalidOperationException($"Exceeded limit of states per frame! Limit: {_maxStatesPerFrame}");
            }

            int localIndex = _frameLengths[_currentFrame];
            int worldIndex = LocalToWorldIndex(localIndex);
            _continuousState[worldIndex] = state;
            _frameLengths[_currentFrame] += 1;
            return new StateHandle<TState>(localIndex, this);
        }

        public ref TState Get(int localIndex)
        {
            int worldIndex = LocalToWorldIndex(localIndex);
            return ref _continuousState[worldIndex];
        }

        public Span<TState> GetAll()
        {
            return new Span<TState>(_continuousState, _frameStarts[_currentFrame], _continuousState.Length);
        }

        public bool Exists(int localIndex)
        {
            return localIndex < _frameLengths[_currentFrame];
        }

        private int LocalToWorldIndex(int localIndex)
        {
            return Loop(_frameStarts[_currentFrame] + localIndex, _continuousState.Length);
        }

        private static int Loop(int a, int b)
		{
            return a % b;
		}

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
