using System;
using System.Runtime.CompilerServices;
using Unity.IL2CPP.CompilerServices;

namespace Massive
{
	[Il2CppSetOption(Option.NullChecks, false)]
	[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
	public class SparseSetFrames
	{
		private readonly int[][] _denseByFrames;
		private readonly int[][] _sparseByFrames;
		private readonly int[] _countByFrames;

		private readonly CyclicFrameCounter _cyclicFrameCounter;

		public SparseSetFrames(int denseCapacity, int sparseCapacity, int framesCapacity = Constants.FramesCapacity)
		{
			_cyclicFrameCounter = new CyclicFrameCounter(framesCapacity);

			_denseByFrames = new int[framesCapacity][];
			_sparseByFrames = new int[framesCapacity][];
			_countByFrames = new int[framesCapacity];

			for (int i = 0; i < framesCapacity; i++)
			{
				_denseByFrames[i] = new int[denseCapacity];
				_sparseByFrames[i] = new int[sparseCapacity];
			}
		}

		public int CurrentFrame => _cyclicFrameCounter.CurrentFrame;

		public int CanRollbackFrames => _cyclicFrameCounter.CanRollbackFrames;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void SaveFrame(SparseSet sparseSet)
		{
			_cyclicFrameCounter.SaveFrame();

			int currentCount = sparseSet.Count;
			int currentFrame = _cyclicFrameCounter.CurrentFrame;

			// Copy everything from current state to current frame
			Array.Copy(sparseSet.Dense, 0, _denseByFrames[currentFrame], 0, currentCount);
			Array.Copy(sparseSet.Sparse, 0, _sparseByFrames[currentFrame], 0, sparseSet.SparseCapacity);
			_countByFrames[currentFrame] = currentCount;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Rollback(int frames, SparseSet sparseSet)
		{
			_cyclicFrameCounter.Rollback(frames);

			// Copy everything from rollback frame to current state
			int rollbackFrame = _cyclicFrameCounter.CurrentFrame;
			int rollbackAliveCount = _countByFrames[rollbackFrame];

			Array.Copy(_denseByFrames[rollbackFrame], 0, sparseSet.Dense, 0, rollbackAliveCount);
			Array.Copy(_sparseByFrames[rollbackFrame], 0, sparseSet.Sparse, 0, sparseSet.SparseCapacity);
			sparseSet.Count = rollbackAliveCount;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void ResizeDense(int capacity)
		{
			for (int i = 0; i < _denseByFrames.Length; i++)
			{
				Array.Resize(ref _denseByFrames[i], capacity);
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void ResizeSparse(int capacity)
		{
			for (int i = 0; i < _sparseByFrames.Length; i++)
			{
				Array.Resize(ref _sparseByFrames[i], capacity);
			}
		}
	}
}
