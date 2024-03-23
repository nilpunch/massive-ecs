using System;
using System.Runtime.CompilerServices;
using Unity.IL2CPP.CompilerServices;

namespace Massive
{
	[Il2CppSetOption(Option.NullChecks, false)]
	[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
	public class SparseSetFrames
	{
		private readonly int[] _denseByFrames;
		private readonly int[] _sparseByFrames;
		private readonly int[] _aliveCountByFrames;

		private readonly SparseSet _sparseSet;
		private readonly CyclicFrameCounter _cyclicFrameCounter;

		public SparseSetFrames(SparseSet sparseSet, int framesCapacity = Constants.FramesCapacity)
		{
			_sparseSet = sparseSet;
			_cyclicFrameCounter = new CyclicFrameCounter(framesCapacity);

			_denseByFrames = new int[framesCapacity * sparseSet.Dense.Length];
			_sparseByFrames = new int[framesCapacity * sparseSet.Sparse.Length];
			_aliveCountByFrames = new int[framesCapacity];
		}

		public int CurrentFrame => _cyclicFrameCounter.CurrentFrame;

		public int CanRollbackFrames => _cyclicFrameCounter.CanRollbackFrames;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void SaveFrame()
		{
			_cyclicFrameCounter.SaveFrame();

			int currentAliveCount = _sparseSet.AliveCount;
			int currentFrame = _cyclicFrameCounter.CurrentFrame;

			// Copy everything from current state to current frame
			Array.Copy(_sparseSet.Dense, 0, _denseByFrames, currentFrame * _sparseSet.Dense.Length, currentAliveCount);
			Array.Copy(_sparseSet.Sparse, 0, _sparseByFrames, currentFrame * _sparseSet.Sparse.Length, _sparseSet.Sparse.Length);
			_aliveCountByFrames[currentFrame] = currentAliveCount;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Rollback(int frames)
		{
			_cyclicFrameCounter.Rollback(frames);

			// Copy everything from rollback frame to current state
			int rollbackFrame = _cyclicFrameCounter.CurrentFrame;
			int rollbackAliveCount = _aliveCountByFrames[rollbackFrame];

			Array.Copy(_denseByFrames, rollbackFrame * _sparseSet.Dense.Length, _sparseSet.Dense, 0, rollbackAliveCount);
			Array.Copy(_sparseByFrames, rollbackFrame * _sparseSet.Sparse.Length, _sparseSet.Sparse, 0, _sparseSet.Sparse.Length);
			_sparseSet.AliveCount = rollbackAliveCount;
		}
	}
}