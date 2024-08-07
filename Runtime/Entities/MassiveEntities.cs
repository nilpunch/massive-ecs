﻿using System;
using System.Runtime.CompilerServices;
using Unity.IL2CPP.CompilerServices;

namespace Massive
{
	[Il2CppSetOption(Option.NullChecks | Option.ArrayBoundsChecks, false)]
	public class MassiveEntities : Entities, IMassive
	{
		private readonly CyclicFrameCounter _cyclicFrameCounter;

		private readonly Entity[][] _denseByFrames;
		private readonly int[][] _sparseByFrames;
		private readonly int[] _maxIdByFrames;
		private readonly int[] _countByFrames;

		public MassiveEntities(int setCapacity = Constants.DefaultSetCapacity, int framesCapacity = Constants.DefaultFramesCapacity)
			: base(setCapacity)
		{
			_cyclicFrameCounter = new CyclicFrameCounter(framesCapacity);

			_denseByFrames = new Entity[framesCapacity][];
			_sparseByFrames = new int[framesCapacity][];
			_maxIdByFrames = new int[framesCapacity];
			_countByFrames = new int[framesCapacity];

			for (int i = 0; i < framesCapacity; i++)
			{
				_denseByFrames[i] = new Entity[Dense.Length];
				_sparseByFrames[i] = new int[Sparse.Length];
			}
		}

		public int CanRollbackFrames => _cyclicFrameCounter.CanRollbackFrames;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void SaveFrame()
		{
			_cyclicFrameCounter.SaveFrame();

			int currentFrame = _cyclicFrameCounter.CurrentFrame;
			int currentCount = Count;
			int currentMaxId = MaxId;

			// Copy everything from current state to current frame
			Array.Copy(Dense, _denseByFrames[currentFrame], currentMaxId);
			Array.Copy(Sparse, _sparseByFrames[currentFrame], currentMaxId);
			_countByFrames[currentFrame] = currentCount;
			_maxIdByFrames[currentFrame] = currentMaxId;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Rollback(int frames)
		{
			_cyclicFrameCounter.Rollback(frames);

			// Copy everything from rollback frame to current state
			int rollbackFrame = _cyclicFrameCounter.CurrentFrame;
			int rollbackCount = _countByFrames[rollbackFrame];
			int rollbackMaxId = _maxIdByFrames[rollbackFrame];

			Array.Copy(_denseByFrames[rollbackFrame], Dense, rollbackMaxId);
			Array.Copy(_sparseByFrames[rollbackFrame], Sparse, rollbackMaxId);
			Count = rollbackCount;
			MaxId = rollbackMaxId;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override void ResizeDense(int capacity)
		{
			base.ResizeDense(capacity);
			for (int i = 0; i < _denseByFrames.Length; i++)
			{
				Array.Resize(ref _denseByFrames[i], capacity);
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override void ResizeSparse(int capacity)
		{
			base.ResizeSparse(capacity);
			for (int i = 0; i < _sparseByFrames.Length; i++)
			{
				Array.Resize(ref _sparseByFrames[i], capacity);
			}
		}
	}
}
