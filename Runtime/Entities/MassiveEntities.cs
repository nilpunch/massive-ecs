using System;
using System.Runtime.CompilerServices;
using Unity.IL2CPP.CompilerServices;

namespace Massive
{
	/// <summary>
	/// Rollback extension for <see cref="Massive.Entities"/>.
	/// </summary>
	[Il2CppSetOption(Option.NullChecks | Option.ArrayBoundsChecks, false)]
	public class MassiveEntities : Entities, IMassive
	{
		private readonly CyclicFrameCounter _cyclicFrameCounter;

		private readonly int[][] _idsByFrames;
		private readonly uint[][] _reusesByFrames;
		private readonly int[][] _sparseByFrames;
		private readonly int[] _maxIdByFrames;
		private readonly int[] _countByFrames;
		private readonly int[] _nextHoleIdByFrames;

		public MassiveEntities(int framesCapacity = Constants.DefaultFramesCapacity)
		{
			_cyclicFrameCounter = new CyclicFrameCounter(framesCapacity);

			_idsByFrames = new int[framesCapacity][];
			_reusesByFrames = new uint[framesCapacity][];
			_sparseByFrames = new int[framesCapacity][];
			_maxIdByFrames = new int[framesCapacity];
			_countByFrames = new int[framesCapacity];
			_nextHoleIdByFrames = new int[framesCapacity];

			for (int i = 0; i < framesCapacity; i++)
			{
				_idsByFrames[i] = new int[Ids.Length];
				_reusesByFrames[i] = new uint[Reuses.Length];
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
			int currentNextHoleId = NextHoleId;

			EnsureCapacityForFrame(currentFrame);

			// Copy everything from current state to current frame
			Array.Copy(Ids, _idsByFrames[currentFrame], currentMaxId);
			Array.Copy(Reuses, _reusesByFrames[currentFrame], currentMaxId);
			Array.Copy(Sparse, _sparseByFrames[currentFrame], currentMaxId);
			_countByFrames[currentFrame] = currentCount;
			_maxIdByFrames[currentFrame] = currentMaxId;
			_nextHoleIdByFrames[currentFrame] = currentNextHoleId;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Rollback(int frames)
		{
			_cyclicFrameCounter.Rollback(frames);

			// Copy everything from rollback frame to current state
			int rollbackFrame = _cyclicFrameCounter.CurrentFrame;
			int rollbackCount = _countByFrames[rollbackFrame];
			int rollbackMaxId = _maxIdByFrames[rollbackFrame];
			int rollbackNextHoleId = _nextHoleIdByFrames[rollbackFrame];

			Array.Copy(_idsByFrames[rollbackFrame], Ids, rollbackMaxId);
			Array.Copy(_reusesByFrames[rollbackFrame], Reuses, rollbackMaxId);
			Array.Copy(_sparseByFrames[rollbackFrame], Sparse, rollbackMaxId);
			Count = rollbackCount;
			MaxId = rollbackMaxId;
			NextHoleId = rollbackNextHoleId;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void EnsureCapacityForFrame(int frame)
		{
			if (_idsByFrames[frame].Length < Ids.Length)
			{
				Array.Resize(ref _idsByFrames[frame], Ids.Length);
			}
			if (_reusesByFrames[frame].Length < Reuses.Length)
			{
				Array.Resize(ref _reusesByFrames[frame], Reuses.Length);
			}
			if (_sparseByFrames[frame].Length < Sparse.Length)
			{
				Array.Resize(ref _sparseByFrames[frame], Sparse.Length);
			}
		}
	}
}
