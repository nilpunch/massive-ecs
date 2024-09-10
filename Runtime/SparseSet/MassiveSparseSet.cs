using System;
using System.Runtime.CompilerServices;
using Unity.IL2CPP.CompilerServices;

namespace Massive
{
	[Il2CppSetOption(Option.NullChecks | Option.ArrayBoundsChecks, false)]
	public class MassiveSparseSet : SparseSet, IMassive
	{
		private readonly CyclicFrameCounter _cyclicFrameCounter;

		private readonly int[][] _denseByFrames;
		private readonly int[][] _sparseByFrames;
		private readonly int[] _countByFrames;

		public MassiveSparseSet(int setCapacity = Constants.DefaultCapacity, int framesCapacity = Constants.DefaultFramesCapacity, bool inPlace = false)
			: base(setCapacity, inPlace)
		{
			_cyclicFrameCounter = new CyclicFrameCounter(framesCapacity);

			_denseByFrames = new int[framesCapacity][];
			_sparseByFrames = new int[framesCapacity][];
			_countByFrames = new int[framesCapacity];

			for (int i = 0; i < framesCapacity; i++)
			{
				_denseByFrames[i] = IsStable ? Array.Empty<int>() : new int[DenseCapacity];
				_sparseByFrames[i] = new int[SparseCapacity];
			}
		}

		public int CanRollbackFrames => _cyclicFrameCounter.CanRollbackFrames;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void SaveFrame()
		{
			_cyclicFrameCounter.SaveFrame();

			int currentFrame = _cyclicFrameCounter.CurrentFrame;
			int currentCount = Count;

			// Copy everything from current state to current frame
			if (IsStable)
			{
				Array.Copy(Sparse, _sparseByFrames[currentFrame], currentCount);
			}
			else
			{
				Array.Copy(Dense, _denseByFrames[currentFrame], currentCount);
				Array.Copy(Sparse, _sparseByFrames[currentFrame], SparseCapacity);
			}
			_countByFrames[currentFrame] = currentCount;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Rollback(int frames)
		{
			_cyclicFrameCounter.Rollback(frames);

			// Copy everything from rollback frame to current state
			int rollbackFrame = _cyclicFrameCounter.CurrentFrame;
			int rollbackCount = _countByFrames[rollbackFrame];

			if (IsStable)
			{
				Array.Copy(_sparseByFrames[rollbackFrame], Sparse, rollbackCount);
				if (rollbackCount < Count)
				{
					Array.Fill(Sparse, Constants.InvalidId, rollbackCount, Count - rollbackCount);
				}
			}
			else
			{
				Array.Copy(_denseByFrames[rollbackFrame], Dense, rollbackCount);
				Array.Copy(_sparseByFrames[rollbackFrame], Sparse, SparseCapacity);
			}
			Count = rollbackCount;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override void ResizeDense(int capacity)
		{
			base.ResizeDense(capacity);

			for (int i = 0; i < _cyclicFrameCounter.FramesCapacity; i++)
			{
				Array.Resize(ref _denseByFrames[i], capacity);
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override void ResizeSparse(int capacity)
		{
			int previousCapacity = SparseCapacity;
			base.ResizeSparse(capacity);

			for (int i = 0; i < _cyclicFrameCounter.FramesCapacity; i++)
			{
				Array.Resize(ref _sparseByFrames[i], capacity);
				if (capacity > previousCapacity)
				{
					Array.Fill(_sparseByFrames[i], Constants.InvalidId, previousCapacity, capacity - previousCapacity);
				}
			}
		}
	}
}
