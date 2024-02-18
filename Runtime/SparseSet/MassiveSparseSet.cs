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
		private readonly int[] _maxDenseByFrames;
		private readonly int[] _maxIdByFrames;
		private readonly int[] _aliveCountByFrames;

		// Current frame
		private readonly int[] _currentDense;
		private readonly int[] _currentSparse;
		private int _currentMaxDense;
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
			_maxDenseByFrames = new int[FramesCapacity];
			_maxIdByFrames = new int[FramesCapacity];
			_aliveCountByFrames = new int[FramesCapacity];

			_currentDense = new int[dataCapacity];
			_currentSparse = new int[dataCapacity];
		}

		public int FramesCapacity { get; }
		public int DataCapacity { get; }

		public int AliveCount => _currentAliveCount;

		// One frame is reserved for restoring
		public int CanRollbackFrames => _savedFrames - 1;

		public MassiveSaveInfo SaveFrame()
		{
			int nextFrame = Loop(_currentFrame + 1, FramesCapacity);
			int currentMaxDense = _currentMaxDense;
			int currentMaxId = _currentMaxId;
			int currentAliveCount = _currentAliveCount;

			int nextFrameIndex = nextFrame * DataCapacity;

			// Copy everything from current frame
			Array.Copy(_currentDense, 0, _denseByFrames, nextFrameIndex, currentMaxDense);
			Array.Copy(_currentSparse, 0, _sparseByFrames, nextFrameIndex, currentMaxId);
			_maxDenseByFrames[nextFrame] = currentMaxDense;
			_maxIdByFrames[nextFrame] = currentMaxId;
			_aliveCountByFrames[nextFrame] = currentAliveCount;

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
			int rollbackMaxDense = _maxDenseByFrames[_currentFrame];
			int rollbackMaxId = _maxIdByFrames[_currentFrame];
			int rollbackAliveCount = _aliveCountByFrames[_currentFrame];

			int rollbackFrameIndex = _currentFrame * DataCapacity;

			// Copy _currentMaxId elements to ensure zeroing excess elements
			Array.Copy(_denseByFrames, rollbackFrameIndex, _currentDense, 0, _currentMaxDense);
			Array.Copy(_sparseByFrames, rollbackFrameIndex, _currentSparse, 0, _currentMaxId);
			_currentMaxDense = rollbackMaxDense;
			_currentMaxId = rollbackMaxId;
			_currentAliveCount = rollbackAliveCount;

			return new MassiveRollbackInfo()
			{
				SourceFrameIndex = rollbackFrameIndex,
				Count = rollbackAliveCount
			};
		}

		public MassiveCreateInfo Ensure(int id)
		{
			if (id >= DataCapacity)
			{
				throw new InvalidOperationException($"Exceeded limit of ids! Limit: {DataCapacity}.");
			}

			int count = _currentAliveCount;
			int dense = _currentSparse[id];

			if (dense < _currentMaxDense)
			{
				if (dense < _currentAliveCount && _currentDense[dense] == id)
				{
					return new MassiveCreateInfo() { Id = id, Dense = dense };
				}

				SwapDense(dense, count);
				_currentAliveCount += 1;
				return new MassiveCreateInfo() { Id = id, Dense = count };
			}

			GrowDense();
			_currentAliveCount += 1;

			if (id >= _currentMaxId)
			{
				_currentMaxId = id + 1;
			}

			// If there are unused elements in the dense,
			// Move the first unused element to the end
			int lastDenseIndex = _currentMaxDense - 1;
			if (count < lastDenseIndex)
			{
				int unusedId = _currentDense[count];
				AssignIndex(unusedId, lastDenseIndex);
			}

			AssignIndex(id, count);

			return new MassiveCreateInfo() { Id = id, Dense = count };
		}

		public MassiveCreateInfo Create()
		{
			int count = _currentAliveCount;

			if (count == DataCapacity)
			{
				throw new InvalidOperationException($"Exceeded limit of data! Limit: {DataCapacity}.");
			}

			// If there are unused elements in the dense array, return last
			if (count < _currentMaxDense)
			{
				_currentAliveCount += 1;
				return new MassiveCreateInfo() { Id = _currentDense[count], Dense = count };
			}

			_currentAliveCount += 1;

			var id = CreateIdForDense(count);
			return new MassiveCreateInfo() { Id = id, Dense = count };
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
			SwapDense(denseIndex, swapDenseIndex);

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

			int swapDenseIndex = aliveCount - 1;
			SwapDense(denseIndex, swapDenseIndex);

			return new MassiveDeleteInfo() { DenseSwapTarget = denseIndex, DenseSwapSource = swapDenseIndex };
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private int CreateIdForDense(int dense)
		{
			int id = _currentMaxId;
			_currentMaxId += 1;
			GrowDense();
			AssignIndex(id, dense);
			return id;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void GrowDense()
		{
			_currentDense[_currentMaxDense] = _currentMaxDense;
			_currentMaxDense += 1;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void SwapDense(int denseA, int denseB)
		{
			int idA = _currentDense[denseA];
			int idB = _currentDense[denseB];
			AssignIndex(idA, denseB);
			AssignIndex(idB, denseA);
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
		public bool IsAliveDense(int dense)
		{
			return dense < _currentAliveCount;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void AssignIndex(int id, int dense)
		{
			_currentSparse[id] = dense;
			_currentDense[dense] = id;
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