using System;
using System.Runtime.CompilerServices;

namespace MassiveData
{
	[Unity.IL2CPP.CompilerServices.Il2CppSetOption(Unity.IL2CPP.CompilerServices.Option.NullChecks, false)]
	[Unity.IL2CPP.CompilerServices.Il2CppSetOption(Unity.IL2CPP.CompilerServices.Option.ArrayBoundsChecks, false)]
	[Unity.IL2CPP.CompilerServices.Il2CppSetOption(Unity.IL2CPP.CompilerServices.Option.DivideByZeroChecks, false)]
	public class MassiveSparseSet
	{
		// Saved frames
		private readonly int[] _denseByFrames;
		private readonly int[] _sparseByFrames;
		private readonly int[] _maxIdByFrames;
		private readonly int[] _aliveCountByFrames;

		// Current frame
		private readonly int[] _currentDense;
		private readonly int[] _currentSparse;
		private int _currentMaxId;
		private int _currentAliveCount;

		private int _currentFrame;
		private int _savedFrames;

		public MassiveSparseSet(int framesCapacity = 120, int dataCapacity = 100)
		{
			// Reserve 1 frame for rollback restoration
			FramesCapacity = framesCapacity + 1;

			DataCapacity = dataCapacity;
			_denseByFrames = new int[FramesCapacity * dataCapacity];
			_sparseByFrames = new int[FramesCapacity * dataCapacity];
			_maxIdByFrames = new int[FramesCapacity];
			_aliveCountByFrames = new int[FramesCapacity];

			_currentDense = new int[dataCapacity];
			_currentSparse = new int[dataCapacity];
		}

		public int FramesCapacity { get; }
		public int DataCapacity { get; }

		public Span<int> Dense => new Span<int>(_denseByFrames, 0, _currentAliveCount);
		public Span<int> Sparse => new Span<int>(_sparseByFrames, 0, _currentAliveCount);

		public int AliveCount => _currentAliveCount;

		// One frame is reserved for restoring
		public int CanRollbackFrames => _savedFrames - 1;

		public MassiveSaveInfo SaveFrame()
		{
			int nextFrame = Loop(_currentFrame + 1, FramesCapacity);
			int currentAliveCount = _currentAliveCount;
			int currentMaxId = _currentMaxId;

			int nextFrameIndex = nextFrame * DataCapacity;

			// Copy everything from current frame
			Array.Copy(_currentDense, 0, _denseByFrames, nextFrameIndex, currentMaxId);
			Array.Copy(_currentSparse, 0, _sparseByFrames, nextFrameIndex, currentMaxId);
			_aliveCountByFrames[nextFrame] = currentAliveCount;
			_maxIdByFrames[nextFrame] = currentMaxId;

			_currentFrame = nextFrame;

			_savedFrames = Math.Min(_savedFrames + 1, FramesCapacity);

			return new MassiveSaveInfo()
			{
				DestinationFrameIndex = nextFrameIndex,
				Count = currentAliveCount
			};
		}

		public MassiveRollbackInfo Rollback(int frames)
		{
			if (frames > CanRollbackFrames)
			{
				throw new InvalidOperationException($"Can't rollback this far. CanRollback:{CanRollbackFrames}, Requested: {frames}.");
			}

			_savedFrames -= frames;
			_currentFrame = LoopNegative(_currentFrame - frames, FramesCapacity);

			// Copy everything from rollback frame to current
			int rollbackAliveCount = _aliveCountByFrames[_currentFrame];
			int rollbackMaxId = _maxIdByFrames[_currentFrame];
			int rollbackFrameIndex = _currentFrame * DataCapacity;

			// Copy _currentMaxId elements to ensure zeroing excess elements
			Array.Copy(_denseByFrames, rollbackFrameIndex, _currentDense, 0, _currentMaxId);
			Array.Copy(_sparseByFrames, rollbackFrameIndex, _currentSparse, 0, _currentMaxId);
			_currentAliveCount = rollbackAliveCount;
			_currentMaxId = rollbackMaxId;

			return new MassiveRollbackInfo()
			{
				SourceFrameIndex = rollbackFrameIndex,
				Count = rollbackAliveCount
			};
		}

		public MassiveCreateInfo Create()
		{
			int count = _currentAliveCount;
			int maxId = _currentMaxId;

			if (count == DataCapacity)
			{
				throw new InvalidOperationException($"Exceeded limit of data! Limit: {DataCapacity}.");
			}

			// If there are unused elements in the dense array, return last
			if (count < maxId)
			{
				_currentAliveCount += 1;
				return new MassiveCreateInfo() { Id = _currentDense[count], Dense = count };
			}

			_currentAliveCount += 1;
			_currentMaxId += 1;

			_currentDense[count] = maxId;
			_currentSparse[maxId] = count;
			return new MassiveCreateInfo() { Id = maxId, Dense = count };
		}

		public MassiveDeleteInfo Delete(int id)
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
				return new MassiveDeleteInfo() { DenseSwapSource = denseIndex, DenseSwapTarget = denseIndex };
			}

			_currentAliveCount -= 1;

			int swapDenseIndex = aliveCount - 1;
			int swapId = _currentDense[swapDenseIndex];

			_currentDense[denseIndex] = swapId;
			_currentSparse[id] = swapDenseIndex;
			_currentDense[swapDenseIndex] = id;
			_currentSparse[swapId] = denseIndex;

			return new MassiveDeleteInfo() { DenseSwapTarget = denseIndex, DenseSwapSource = swapDenseIndex };
		}

		public MassiveDeleteInfo DeleteDense(int denseIndex)
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
				return new MassiveDeleteInfo() { DenseSwapSource = denseIndex, DenseSwapTarget = denseIndex };
			}

			_currentAliveCount -= 1;

			int deleteId = _currentDense[denseIndex];
			int swapDenseIndex = aliveCount - 1;
			int swapId = _currentDense[swapDenseIndex];

			_currentDense[denseIndex] = swapId;
			_currentSparse[deleteId] = swapDenseIndex;
			_currentDense[swapDenseIndex] = deleteId;
			_currentSparse[swapId] = denseIndex;

			return new MassiveDeleteInfo() { DenseSwapTarget = denseIndex, DenseSwapSource = swapDenseIndex };
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public int GetDenseIndex(int id)
		{
			return _currentSparse[id];
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool IsAlive(int id)
		{
			if (id >= DataCapacity)
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