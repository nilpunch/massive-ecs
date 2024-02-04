using System;
using System.Runtime.CompilerServices;

namespace Massive
{
	[Unity.IL2CPP.CompilerServices.Il2CppSetOption(Unity.IL2CPP.CompilerServices.Option.NullChecks, false)]
	[Unity.IL2CPP.CompilerServices.Il2CppSetOption(Unity.IL2CPP.CompilerServices.Option.ArrayBoundsChecks, false)]
	[Unity.IL2CPP.CompilerServices.Il2CppSetOption(Unity.IL2CPP.CompilerServices.Option.DivideByZeroChecks, false)]
	public class MassiveData<T> : IMassiveData where T : struct
	{
		private readonly int _framesCapacity;
		private readonly int _dataCapacity;

		// Saved frames
		private readonly T[] _dataByFrames;
		private readonly int[] _denseByFrames;
		private readonly int[] _sparseByFrames;
		private readonly int[] _maxIdByFrames;
		private readonly int[] _aliveCountByFrames;

		// Current frame
		private readonly T[] _currentData;
		private readonly int[] _currentDense;
		private readonly int[] _currentSparse;
		private int _currentMaxId;
		private int _currentAliveCount;

		private int _currentFrame;
		private int _savedFrames;

		public MassiveData(int framesCapacity = 120, int dataCapacity = 100)
		{
			// Reserve 1 frame for rollback restoration
			_framesCapacity = framesCapacity + 1;

			_dataCapacity = dataCapacity;
			_dataByFrames = new T[_framesCapacity * dataCapacity];
			_denseByFrames = new int[_framesCapacity * dataCapacity];
			_sparseByFrames = new int[_framesCapacity * dataCapacity];
			_maxIdByFrames = new int[_framesCapacity];
			_aliveCountByFrames = new int[_framesCapacity];

			_currentData = new T[dataCapacity];
			_currentDense = new int[dataCapacity];
			_currentSparse = new int[dataCapacity];
		}

		public Span<T> Data => new Span<T>(_currentData);
		public Span<int> Dense => new Span<int>(_denseByFrames);
		public Span<int> Sparse => new Span<int>(_sparseByFrames);

		public int AliveCount => _currentAliveCount;

		// One frame is reserved for restoring
		public int CanRollbackFrames => _savedFrames - 1;

		public void SaveFrame()
		{
			int nextFrame = Loop(_currentFrame + 1, _framesCapacity);
			int currentAliveCount = _currentAliveCount;
			int currentMaxId = _currentMaxId;

			int nextFrameIndex = nextFrame * _dataCapacity;

			// Copy everything from current frame
			Array.Copy(_currentData, 0, _dataByFrames, nextFrameIndex, currentAliveCount);
			Array.Copy(_currentDense, 0, _denseByFrames, nextFrameIndex, currentMaxId);
			Array.Copy(_currentSparse, 0, _sparseByFrames, nextFrameIndex, currentMaxId);
			_aliveCountByFrames[nextFrame] = currentAliveCount;
			_maxIdByFrames[nextFrame] = currentMaxId;

			_currentFrame = nextFrame;

			_savedFrames = Math.Min(_savedFrames + 1, _framesCapacity);
		}

		public void Rollback(int frames)
		{
			if (frames > CanRollbackFrames)
			{
				throw new InvalidOperationException($"Can't rollback this far. CanRollback:{CanRollbackFrames}, Requested: {frames}.");
			}

			_savedFrames -= frames;
			_currentFrame = LoopNegative(_currentFrame - frames, _framesCapacity);

			// Copy everything from rollback frame to current
			int rollbackAliveCount = _aliveCountByFrames[_currentFrame];
			int rollbackMaxId = _maxIdByFrames[_currentFrame];
			int rollbackFrameIndex = _currentFrame * _dataCapacity;
			Array.Copy(_dataByFrames, rollbackFrameIndex, _currentData, 0, rollbackAliveCount);

			// Copy _currentMaxId elements to ensure zeroing excess elements
			Array.Copy(_denseByFrames, rollbackFrameIndex, _currentDense, 0, _currentMaxId);
			Array.Copy(_sparseByFrames, rollbackFrameIndex, _currentSparse, 0, _currentMaxId);
			_currentAliveCount = rollbackAliveCount;
			_currentMaxId = rollbackMaxId;
		}

		public int Create(T data = default)
		{
			int count = _currentAliveCount;
			int maxId = _currentMaxId;

			if (count == _dataCapacity)
			{
				throw new InvalidOperationException($"Exceeded limit of data! Limit: {_dataCapacity}.");
			}

			_currentData[count] = data;

			// If there are unused elements in the dense array, return last
			if (count < maxId)
			{
				_currentAliveCount += 1;
				return _currentDense[count];
			}

			_currentAliveCount += 1;
			_currentMaxId += 1;

			_currentDense[count] = maxId;
			_currentSparse[maxId] = count;
			return maxId;
		}

		public void Delete(int id)
		{
			int aliveCount = _currentAliveCount;
			int denseIndex = _currentSparse[id];

			if (denseIndex >= aliveCount)
			{
				throw new InvalidOperationException($"Id is not alive! Id: {id}.");
			}

			// If dense is the last used element, simply decrease count
			if (denseIndex == aliveCount - 1)
			{
				_currentAliveCount -= 1;
				return;
			}

			_currentAliveCount -= 1;

			int swapDenseIndex = aliveCount - 1;
			int swapId = _currentDense[swapDenseIndex];

			_currentData[denseIndex] = _currentData[swapDenseIndex];

			_currentDense[denseIndex] = swapId;
			_currentSparse[id] = swapDenseIndex;
			_currentDense[swapDenseIndex] = id;
			_currentSparse[swapId] = denseIndex;
		}

		public void DeleteDense(int denseIndex)
		{
			int aliveCount = _currentAliveCount;

			if (denseIndex >= aliveCount)
			{
				throw new InvalidOperationException($"Dense is not alive! Dense: {denseIndex}.");
			}

			// If dense is the last used element, simply decrease count
			if (denseIndex == aliveCount - 1)
			{
				_currentAliveCount -= 1;
				return;
			}

			_currentAliveCount -= 1;

			int deleteId = _currentDense[denseIndex];
			int swapDenseIndex = aliveCount - 1;
			int swapId = _currentDense[swapDenseIndex];

			_currentData[denseIndex] = _currentData[swapDenseIndex];

			_currentDense[denseIndex] = swapId;
			_currentSparse[deleteId] = swapDenseIndex;
			_currentDense[swapDenseIndex] = deleteId;
			_currentSparse[swapId] = denseIndex;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public ref T Get(int id)
		{
			return ref _currentData[_currentSparse[id]];
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool IsAlive(int id)
		{
			if (id >= _dataCapacity)
				return false;

			int denseIndex = _currentSparse[id];

			return denseIndex < _currentAliveCount && _currentDense[denseIndex] == id;
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