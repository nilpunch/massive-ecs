using System;
using System.Runtime.CompilerServices;
using Unity.IL2CPP.CompilerServices;

namespace Massive
{
	[Il2CppSetOption(Option.NullChecks | Option.ArrayBoundsChecks, false)]
	public class MassiveEntities : Entities, IMassive
	{
		private readonly CyclicFrameCounter _cyclicFrameCounter;

		private readonly int[][] _idsByFrames;
		private readonly uint[][] _reusesByFrames;
		private readonly int[][] _sparseByFrames;
		private readonly int[] _maxIdByFrames;
		private readonly int[] _countByFrames;

		public MassiveEntities(int setCapacity = Constants.DefaultCapacity, int framesCapacity = Constants.DefaultFramesCapacity)
			: base(setCapacity)
		{
			_cyclicFrameCounter = new CyclicFrameCounter(framesCapacity);

			_idsByFrames = new int[framesCapacity][];
			_reusesByFrames = new uint[framesCapacity][];
			_sparseByFrames = new int[framesCapacity][];
			_maxIdByFrames = new int[framesCapacity];
			_countByFrames = new int[framesCapacity];

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

			// Copy everything from current state to current frame
			Array.Copy(Ids, _idsByFrames[currentFrame], currentMaxId);
			Array.Copy(Reuses, _reusesByFrames[currentFrame], currentMaxId);
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

			Array.Copy(_idsByFrames[rollbackFrame], Ids, rollbackMaxId);
			Array.Copy(_reusesByFrames[rollbackFrame], Reuses, rollbackMaxId);
			Array.Copy(_sparseByFrames[rollbackFrame], Sparse, rollbackMaxId);
			Count = rollbackCount;
			MaxId = rollbackMaxId;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override void ResizeDense(int capacity)
		{
			base.ResizeDense(capacity);
			for (int i = 0; i < _idsByFrames.Length; i++)
			{
				Array.Resize(ref _idsByFrames[i], capacity);
			}
			for (int i = 0; i < _reusesByFrames.Length; i++)
			{
				Array.Resize(ref _reusesByFrames[i], capacity);
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
