using System;
using System.Runtime.CompilerServices;

namespace Massive
{
	public class SparseSetFrames
	{
		private readonly int[] _denseByFrames;
		private readonly int[] _sparseByFrames;
		private readonly int[] _aliveCountByFrames;

		private readonly SparseSet _sparseSet;
		private readonly int _framesCapacity;
		private int _currentFrame;
		private int _savedFrames;

		public SparseSetFrames(SparseSet sparseSet, int framesCapacity = Constants.FramesCapacity)
		{
			_sparseSet = sparseSet;
			_framesCapacity = framesCapacity;

			_denseByFrames = new int[framesCapacity * sparseSet.Dense.Length];
			_sparseByFrames = new int[framesCapacity * sparseSet.Sparse.Length];
			_aliveCountByFrames = new int[framesCapacity];
		}

		public int CurrentFrame => _currentFrame;

		public int CanRollbackFrames => _savedFrames - 1;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void SaveFrame()
		{
			int currentAliveCount = _sparseSet.AliveCount;
			int nextFrame = Loop(_currentFrame + 1, _framesCapacity);

			// Copy everything from current state to next frame
			Array.Copy(_sparseSet.Dense, 0, _denseByFrames, nextFrame * _sparseSet.Dense.Length, currentAliveCount);
			Array.Copy(_sparseSet.Sparse, 0, _sparseByFrames, nextFrame * _sparseSet.Sparse.Length, _sparseSet.Sparse.Length);
			_aliveCountByFrames[nextFrame] = currentAliveCount;

			_currentFrame = nextFrame;
			_savedFrames = Math.Min(_savedFrames + 1, _framesCapacity);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Rollback(int frames)
		{
			if (frames > CanRollbackFrames)
			{
				throw new ArgumentOutOfRangeException(nameof(frames), frames, $"Can't rollback this far. CanRollbackFrames: {CanRollbackFrames}.");
			}

			_savedFrames -= frames;
			_currentFrame = LoopNegative(_currentFrame - frames, _framesCapacity);

			// Copy everything from rollback frame to current state
			int rollbackAliveCount = _aliveCountByFrames[_currentFrame];
			int rollbackFrame = _currentFrame;

			Array.Copy(_denseByFrames, rollbackFrame * _sparseSet.Dense.Length, _sparseSet.Dense, 0, rollbackAliveCount);
			Array.Copy(_sparseByFrames, rollbackFrame * _sparseSet.Sparse.Length, _sparseSet.Sparse, 0, _sparseSet.Sparse.Length);
			_sparseSet.AliveCount = rollbackAliveCount;
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