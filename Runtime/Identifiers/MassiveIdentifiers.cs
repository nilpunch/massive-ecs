using System;
using System.Runtime.CompilerServices;
using Unity.IL2CPP.CompilerServices;

namespace Massive
{
	[Il2CppSetOption(Option.NullChecks, false)]
	[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
	public class MassiveIdentifiers : Identifiers, IMassive
	{
		private readonly Identifier[][] _denseByFrames;
		private readonly int[][] _sparseByFrames;
		private readonly int[] _maxIdByFrames;
		private readonly int[] _aliveCountByFrames;

		private readonly CyclicFrameCounter _cyclicFrameCounter;

		public MassiveIdentifiers(int dataCapacity = Constants.DataCapacity, int framesCapacity = Constants.FramesCapacity)
			: base(dataCapacity)
		{
			_cyclicFrameCounter = new CyclicFrameCounter(framesCapacity);

			_denseByFrames = new Identifier[framesCapacity][];
			_sparseByFrames = new int[framesCapacity][];
			_maxIdByFrames = new int[framesCapacity];
			_aliveCountByFrames = new int[framesCapacity];

			for (int i = 0; i < framesCapacity; i++)
			{
				_denseByFrames[i] = new Identifier[Dense.Length];
				_sparseByFrames[i] = new int[Sparse.Length];
			}
		}

		public int CanRollbackFrames => _cyclicFrameCounter.CanRollbackFrames;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void SaveFrame()
		{
			_cyclicFrameCounter.SaveFrame();

			int currentAliveCount = AliveCount;
			int currentMaxId = MaxId;
			int currentFrame = _cyclicFrameCounter.CurrentFrame;

			// Copy everything from current state to current frame
			Array.Copy(Dense, 0, _denseByFrames[currentFrame], 0, currentMaxId);
			Array.Copy(Sparse, 0, _sparseByFrames[currentFrame], 0, currentMaxId);
			_aliveCountByFrames[currentFrame] = currentAliveCount;
			_maxIdByFrames[currentFrame] = currentMaxId;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Rollback(int frames)
		{
			_cyclicFrameCounter.Rollback(frames);

			// Copy everything from rollback frame to current state
			int rollbackFrame = _cyclicFrameCounter.CurrentFrame;
			int rollbackAliveCount = _aliveCountByFrames[rollbackFrame];
			int rollbackMaxId = _maxIdByFrames[rollbackFrame];

			Array.Copy(_denseByFrames[rollbackFrame], 0, Dense, 0, rollbackMaxId);
			Array.Copy(_sparseByFrames[rollbackFrame], 0, Sparse, 0, rollbackMaxId);
			AliveCount = rollbackAliveCount;
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