using System;
using System.Runtime.CompilerServices;
using Unity.IL2CPP.CompilerServices;

namespace Massive
{
	[Il2CppSetOption(Option.NullChecks | Option.ArrayBoundsChecks, false)]
	public class MassiveSparseSet : SparseSet, IMassive
	{
		private readonly CyclicFrameCounter _cyclicFrameCounter;

		private readonly int[][] _packedByFrames;
		private readonly int[][] _sparseByFrames;
		private readonly int[] _countByFrames;

		public MassiveSparseSet(int setCapacity = Constants.DefaultCapacity, int framesCapacity = Constants.DefaultFramesCapacity)
			: base(setCapacity)
		{
			_cyclicFrameCounter = new CyclicFrameCounter(framesCapacity);

			_packedByFrames = new int[framesCapacity][];
			_sparseByFrames = new int[framesCapacity][];
			_countByFrames = new int[framesCapacity];

			for (int i = 0; i < framesCapacity; i++)
			{
				_packedByFrames[i] = new int[PackedCapacity];
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
			Array.Copy(Packed, _packedByFrames[currentFrame], currentCount);
			Array.Copy(Sparse, _sparseByFrames[currentFrame], SparseCapacity);
			_countByFrames[currentFrame] = currentCount;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Rollback(int frames)
		{
			_cyclicFrameCounter.Rollback(frames);

			// Copy everything from rollback frame to current state
			int rollbackFrame = _cyclicFrameCounter.CurrentFrame;
			int rollbackCount = _countByFrames[rollbackFrame];

			Array.Copy(_packedByFrames[rollbackFrame], Packed, rollbackCount);
			Array.Copy(_sparseByFrames[rollbackFrame], Sparse, SparseCapacity);
			Count = rollbackCount;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override void ResizePacked(int capacity)
		{
			base.ResizePacked(capacity);

			for (int i = 0; i < _cyclicFrameCounter.FramesCapacity; i++)
			{
				Array.Resize(ref _packedByFrames[i], capacity);
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
