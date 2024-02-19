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

			// Reserve 1 for the first element
			DataCapacity = dataCapacity + 1;
			_denseByFrames = new int[FramesCapacity * DataCapacity];
			_sparseByFrames = new int[FramesCapacity * DataCapacity];
			_maxDenseByFrames = new int[FramesCapacity];
			_maxIdByFrames = new int[FramesCapacity];
			_aliveCountByFrames = new int[FramesCapacity];

			_currentDense = new int[DataCapacity];
			_currentSparse = new int[DataCapacity];

			// Consume first value in dense array as 0 is used in the sparse array to
			// indicate that a sparse element hasn't been paired yet
			_currentMaxDense = 1;
			_currentAliveCount = 1;
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

			// Check if element is paired
			if (dense != 0)
			{
				if (dense < _currentAliveCount)
				{
					// If dense is already alive, nothing to be done
					return new MassiveCreateInfo() { Id = id, Dense = dense };
				}

				// If dense is not alive, swap it with the first unused element
				SwapDense(dense, count);

				// First unused element is now last used element
				_currentAliveCount += 1;

				return new MassiveCreateInfo() { Id = id, Dense = count };
			}

			// Add new element to dense array and pair it with sparse
			_currentMaxDense += 1;
			_currentAliveCount += 1;
			// If index is larger than max id, update max id
			_currentMaxId = Math.Max(_currentMaxId, id + 1);

			// If there are unused elements in the dense,
			// move the first unused element to the end
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
			var maxDense = _currentMaxDense;
			if (count < maxDense)
			{
				_currentAliveCount += 1;
				return new MassiveCreateInfo() { Id = _currentDense[count], Dense = count };
			}

			var maxId = _currentMaxId;
			if (maxId == DataCapacity)
			{
				throw new InvalidOperationException($"Exceeded limit of ids! Limit: {DataCapacity}.");
			}

			// Add new element to dense array and pair it with a new element from sparse
			_currentAliveCount += 1;
			_currentMaxId += 1;
			_currentMaxDense += 1;

			AssignIndex(maxId, maxDense);

			return new MassiveCreateInfo() { Id = maxId, Dense = maxDense };
		}

		public MassiveDeleteInfo? Delete(int id)
		{
			int aliveCount = _currentAliveCount;
			int dense = _currentSparse[id];

			// Element is not paired or not alive, nothing to be done
			if (dense == 0 || dense >= aliveCount)
			{
				return null;
			}

			_currentAliveCount -= 1;

			// If dense is the last used element, nothing to be done
			if (dense == _currentAliveCount)
			{
				return null;
			}

			int swapDense = _currentAliveCount;
			SwapDense(dense, swapDense);

			return new MassiveDeleteInfo() { DenseSwapTarget = dense, DenseSwapSource = swapDense };
		}

		public MassiveDeleteInfo? DeleteDense(int dense)
		{
			int aliveCount = _currentAliveCount;

			//  Element is not paired or not alive, nothing to be done
			if (dense == 0 || dense >= aliveCount)
			{
				return null;
			}

			_currentAliveCount -= 1;

			// If dense is the last used element, nothing to be done
			if (dense == aliveCount - 1)
			{
				return null;
			}

			int swapDense = _currentAliveCount;
			SwapDense(dense, swapDense);

			return new MassiveDeleteInfo() { DenseSwapTarget = dense, DenseSwapSource = swapDense };
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public int GetDenseIndex(int id)
		{
			return _currentSparse[id];
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool IsAlive(int id)
		{
			int dense = _currentSparse[id];
			return dense != 0 && dense < _currentAliveCount && _currentDense[dense] == id;
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