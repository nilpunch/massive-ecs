using System;
using System.Runtime.CompilerServices;
using Unity.IL2CPP.CompilerServices;

namespace Massive
{
	[Il2CppSetOption(Option.NullChecks, false)]
	[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
	public class MassiveSparseSet : SparseSet, IMassive
	{
		private readonly CyclicFrameCounter _cyclicFrameCounter;

		private readonly PackedArray<int>[] _denseByFrames;
		private readonly PackedArray<int>[] _sparseByFrames;
		private readonly int[] _countByFrames;

		public MassiveSparseSet(int dataCapacity = Constants.DataCapacity, int framesCapacity = Constants.FramesCapacity)
			: base(dataCapacity)
		{
			_cyclicFrameCounter = new CyclicFrameCounter(framesCapacity);

			_denseByFrames = new PackedArray<int>[framesCapacity];
			_sparseByFrames = new PackedArray<int>[framesCapacity];
			_countByFrames = new int[framesCapacity];

			for (int i = 0; i < framesCapacity; i++)
			{
				_denseByFrames[i] = new PackedArray<int>();
				_sparseByFrames[i] = new PackedArray<int>();
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
			Dense.CopyTo(_denseByFrames[currentFrame], currentCount);
			Sparse.CopyTo(_sparseByFrames[currentFrame]);
			_countByFrames[currentFrame] = currentCount;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Rollback(int frames)
		{
			_cyclicFrameCounter.Rollback(frames);

			// Copy everything from rollback frame to current state
			int rollbackFrame = _cyclicFrameCounter.CurrentFrame;
			int rollbackCount = _countByFrames[rollbackFrame];

			_denseByFrames[rollbackFrame].CopyTo(Dense, rollbackCount);
			_sparseByFrames[rollbackFrame].CopyTo(Sparse);
			Count = rollbackCount;
		}
	}
}
