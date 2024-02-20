using System;
using System.Runtime.CompilerServices;

namespace MassiveData
{
	[Unity.IL2CPP.CompilerServices.Il2CppSetOption(Unity.IL2CPP.CompilerServices.Option.NullChecks, false)]
	[Unity.IL2CPP.CompilerServices.Il2CppSetOption(Unity.IL2CPP.CompilerServices.Option.ArrayBoundsChecks, false)]
	[Unity.IL2CPP.CompilerServices.Il2CppSetOption(Unity.IL2CPP.CompilerServices.Option.DivideByZeroChecks, false)]
	public class MassiveSparseSet : IMassive
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

		private readonly int _framesCapacity;
		private int _currentFrame;
		private int _savedFrames;

		public MassiveSparseSet(int framesCapacity = 121, int dataCapacity = 100)
		{
			_framesCapacity = framesCapacity;

			_currentDense = new int[dataCapacity];
			_currentSparse = new int[dataCapacity];

			_denseByFrames = new int[_framesCapacity * _currentDense.Length];
			_sparseByFrames = new int[_framesCapacity * _currentSparse.Length];
			_maxDenseByFrames = new int[_framesCapacity];
			_maxIdByFrames = new int[_framesCapacity];
			_aliveCountByFrames = new int[_framesCapacity];
		}

		public int CurrentFrame => _currentFrame;

		/// <summary>
		/// Dense elements count.
		/// </summary>
		public int AliveCount => _currentAliveCount;

		/// <summary>
		/// Can be negative, when there absolutely no saved frames to restore information.
		/// </summary>
		public int CanRollbackFrames => _savedFrames - 1;

		public void SaveFrame()
		{
			int currentMaxDense = _currentMaxDense;
			int currentMaxId = _currentMaxId;
			int currentAliveCount = _currentAliveCount;
			int nextFrame = Loop(_currentFrame + 1, _framesCapacity);

			// Copy everything from current frame
			Array.Copy(_currentDense, 0, _denseByFrames, nextFrame * _currentDense.Length, currentMaxDense);
			Array.Copy(_currentSparse, 0, _sparseByFrames, nextFrame * _currentSparse.Length, currentMaxId);
			_maxDenseByFrames[nextFrame] = currentMaxDense;
			_maxIdByFrames[nextFrame] = currentMaxId;
			_aliveCountByFrames[nextFrame] = currentAliveCount;

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
			int rollbackMaxDense = _maxDenseByFrames[_currentFrame];
			int rollbackMaxId = _maxIdByFrames[_currentFrame];
			int rollbackAliveCount = _aliveCountByFrames[_currentFrame];
			int rollbackFrame = _currentFrame;

			// Copy _currentMaxId elements to ensure zeroing excess elements
			Array.Copy(_denseByFrames, rollbackFrame * _currentDense.Length, _currentDense, 0, _currentMaxDense);
			Array.Copy(_sparseByFrames, rollbackFrame * _currentSparse.Length, _currentSparse, 0, _currentMaxId);
			_currentMaxDense = rollbackMaxDense;
			_currentMaxId = rollbackMaxId;
			_currentAliveCount = rollbackAliveCount;
		}

		public MassiveCreateInfo Ensure(int id)
		{
			if (id >= _currentSparse.Length)
			{
				throw new InvalidOperationException($"Exceeded limit of ids! Limit: {_currentSparse.Length}.");
			}

			int count = _currentAliveCount;
			int dense = _currentSparse[id];

			// Check if element is paired
			if (_currentDense[dense] == id)
			{
				// If dense is already alive, nothing to be done
				if (dense < count)
				{
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
			_currentMaxId = Math.Max(_currentMaxId, id + 1);

			// If there are unused elements in the dense,
			// move the first unused element to the end
			int lastDense = _currentMaxDense - 1;
			if (count < lastDense)
			{
				int unusedId = _currentDense[count];
				AssignIndex(unusedId, lastDense);
			}

			AssignIndex(id, count);

			return new MassiveCreateInfo() { Id = id, Dense = count };
		}

		public MassiveCreateInfo Create()
		{
			int count = _currentAliveCount;
			if (count == _currentDense.Length)
			{
				throw new InvalidOperationException($"Exceeded limit of data! Limit: {_currentDense.Length}.");
			}

			// If there are unused elements in the dense array, return last
			var maxDense = _currentMaxDense;
			if (count < maxDense)
			{
				_currentAliveCount += 1;
				return new MassiveCreateInfo() { Id = _currentDense[count], Dense = count };
			}

			var maxId = _currentMaxId;
			if (maxId == _currentSparse.Length)
			{
				throw new InvalidOperationException($"Exceeded limit of ids! Limit: {_currentSparse.Length}.");
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

			// If element is not alive, nothing to be done
			if (!TryGetDense(id, out var dense))
			{
				return default;
			}

			_currentAliveCount -= 1;

			// If dense is the last used element, nothing to be done
			if (dense == aliveCount - 1)
			{
				return default;
			}

			int swapDense = aliveCount - 1;
			SwapDense(dense, swapDense);

			return new MassiveDeleteInfo() { DenseSwapTarget = dense, DenseSwapSource = swapDense };
		}

		public MassiveDeleteInfo? DeleteDense(int dense)
		{
			int aliveCount = _currentAliveCount;

			// Element is not alive, nothing to be done
			if (dense >= aliveCount)
			{
				return default;
			}

			_currentAliveCount -= 1;

			// If dense is the last used element, nothing to be done
			if (dense == aliveCount - 1)
			{
				return default;
			}

			int swapDense = aliveCount - 1;
			SwapDense(dense, swapDense);

			return new MassiveDeleteInfo() { DenseSwapTarget = dense, DenseSwapSource = swapDense };
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public int GetDense(int id)
		{
			return _currentSparse[id];
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool IsAlive(int id)
		{
			if (id >= _currentMaxId)
				return false;

			int dense = _currentSparse[id];

			return dense < _currentAliveCount && _currentDense[dense] == id;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool TryGetDense(int id, out int dense)
		{
			if (id >= _currentMaxId)
			{
				dense = default;
				return false;
			}

			dense = _currentSparse[id];

			return dense < _currentAliveCount && _currentDense[dense] == id;
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