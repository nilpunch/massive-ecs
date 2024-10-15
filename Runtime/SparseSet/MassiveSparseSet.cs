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

		public MassiveSparseSet(int setCapacity = Constants.DefaultCapacity, int framesCapacity = Constants.DefaultFramesCapacity, IndexingMode indexingMode = IndexingMode.Packed)
			: base(setCapacity, indexingMode)
		{
			_cyclicFrameCounter = new CyclicFrameCounter(framesCapacity);

			_packedByFrames = new int[framesCapacity][];
			_sparseByFrames = new int[framesCapacity][];
			_countByFrames = new int[framesCapacity];

			for (int i = 0; i < framesCapacity; i++)
			{
				_packedByFrames[i] = IsPacked ? new int[PackedCapacity] : Array.Empty<int>();
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

			EnsureCapacityForFrame(currentFrame);

			// Copy everything from current state to current frame
			if (IsPacked)
			{
				Array.Copy(Packed, _packedByFrames[currentFrame], currentCount);
				Array.Copy(Sparse, _sparseByFrames[currentFrame], SparseCapacity);
			}
			else
			{
				Array.Copy(Sparse, _sparseByFrames[currentFrame], currentCount);
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

			if (IsPacked)
			{
				int rollbackSparseCapacity = _sparseByFrames[rollbackFrame].Length;
				Array.Copy(_packedByFrames[rollbackFrame], Packed, rollbackCount);
				Array.Copy(_sparseByFrames[rollbackFrame], Sparse, rollbackSparseCapacity);
				if (rollbackSparseCapacity < SparseCapacity)
				{
					Array.Fill(Sparse, Constants.InvalidId, rollbackSparseCapacity, SparseCapacity - rollbackSparseCapacity);
				}
			}
			else
			{
				Array.Copy(_sparseByFrames[rollbackFrame], Sparse, rollbackCount);
				if (rollbackCount < Count)
				{
					Array.Fill(Sparse, Constants.InvalidId, rollbackCount, Count - rollbackCount);
				}
			}
			Count = rollbackCount;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void EnsureCapacityForFrame(int frame)
		{
			if (_sparseByFrames[frame].Length < SparseCapacity)
			{
				var previousCapacity = _sparseByFrames[frame].Length;
				Array.Resize(ref _sparseByFrames[frame], SparseCapacity);
				Array.Fill(_sparseByFrames[frame], Constants.InvalidId, previousCapacity, SparseCapacity - previousCapacity);
			}

			if (_packedByFrames[frame].Length < PackedCapacity)
			{
				Array.Resize(ref _packedByFrames[frame], PackedCapacity);
			}
		}
	}
}
