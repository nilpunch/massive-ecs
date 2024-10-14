using System.Collections.Generic;

namespace Massive
{
	public class MassiveOwningGroup : OwningGroup, IMassive
	{
		private readonly CyclicFrameCounter _cyclicFrameCounter;

		private readonly int[] _lengthByFrames;
		private readonly bool[] _syncedByFrames;

		public MassiveOwningGroup(IReadOnlyList<SparseSet> owned, IReadOnlyList<SparseSet> include = null,
			IReadOnlyList<SparseSet> exclude = null, int framesCapacity = Constants.DefaultFramesCapacity)
			: base(owned, include, exclude)
		{
			_cyclicFrameCounter = new CyclicFrameCounter(framesCapacity);

			_lengthByFrames = new int[framesCapacity];
			_syncedByFrames = new bool[framesCapacity];
		}

		public int CanRollbackFrames => _cyclicFrameCounter.CanRollbackFrames;

		public void SaveFrame()
		{
			_cyclicFrameCounter.SaveFrame();

			var currentFrame = _cyclicFrameCounter.CurrentFrame;

			_lengthByFrames[currentFrame] = Count;
			_syncedByFrames[currentFrame] = IsSynced;
		}

		public void Rollback(int frames)
		{
			_cyclicFrameCounter.Rollback(frames);

			var rollbackFrame = _cyclicFrameCounter.CurrentFrame;

			Count = _lengthByFrames[rollbackFrame];
			IsSynced = _syncedByFrames[rollbackFrame];
		}
	}
}
