﻿using System;
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
		private readonly int[] _nextHoleByFrames;

		public MassiveSparseSet(int setCapacity = Constants.DefaultCapacity, int framesCapacity = Constants.DefaultFramesCapacity, PackingMode packingMode = PackingMode.Continuous)
			: base(setCapacity, packingMode)
		{
			_cyclicFrameCounter = new CyclicFrameCounter(framesCapacity);

			_packedByFrames = new int[framesCapacity][];
			_sparseByFrames = new int[framesCapacity][];
			_countByFrames = new int[framesCapacity];
			_nextHoleByFrames = new int[framesCapacity];

			for (int i = 0; i < framesCapacity; i++)
			{
				_packedByFrames[i] = Array.Empty<int>();
				_sparseByFrames[i] = Array.Empty<int>();
			}
		}

		public int CanRollbackFrames => _cyclicFrameCounter.CanRollbackFrames;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void SaveFrame()
		{
			_cyclicFrameCounter.SaveFrame();

			int currentFrame = _cyclicFrameCounter.CurrentFrame;
			int currentCount = Count;
			int currentNextHole = NextHole;

			EnsureCapacityForFrame(currentFrame);

			// Copy everything from current state to current frame
			Array.Copy(Packed, _packedByFrames[currentFrame], currentCount);
			Array.Copy(Sparse, _sparseByFrames[currentFrame], SparseCapacity);
			_countByFrames[currentFrame] = currentCount;
			_nextHoleByFrames[currentFrame] = currentNextHole;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Rollback(int frames)
		{
			_cyclicFrameCounter.Rollback(frames);

			// Copy everything from rollback frame to current state
			int rollbackFrame = _cyclicFrameCounter.CurrentFrame;
			int rollbackCount = _countByFrames[rollbackFrame];
			int rollbackSparseCapacity = _sparseByFrames[rollbackFrame].Length;
			int rollbackNextHole = _nextHoleByFrames[rollbackFrame];

			Array.Copy(_packedByFrames[rollbackFrame], Packed, rollbackCount);
			Array.Copy(_sparseByFrames[rollbackFrame], Sparse, rollbackSparseCapacity);
			if (rollbackSparseCapacity < SparseCapacity)
			{
				Array.Fill(Sparse, Constants.InvalidId, rollbackSparseCapacity, SparseCapacity - rollbackSparseCapacity);
			}
			Count = rollbackCount;
			NextHole = rollbackNextHole;
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
