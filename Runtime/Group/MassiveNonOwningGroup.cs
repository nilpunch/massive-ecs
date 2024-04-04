namespace Massive
{
	public class MassiveNonOwningGroup : NonOwningGroup, IMassive
	{
		private readonly IMassive _massiveGroup;
		private readonly CyclicFrameCounter _cyclicFrameCounter;

		private readonly bool[] _syncedByFrames;

		public MassiveNonOwningGroup(IReadOnlySet[] include, IReadOnlySet[] exclude = null, int dataCapacity = Constants.DataCapacity,
			int framesCapacity = Constants.FramesCapacity)
			: base(new MassiveSparseSet(dataCapacity, framesCapacity), include, exclude)
		{
			// Fetch instance from base
			_massiveGroup = (IMassive)GroupSet;

			_cyclicFrameCounter = new CyclicFrameCounter(framesCapacity);

			_syncedByFrames = new bool[framesCapacity];
		}

		public int CanRollbackFrames => _massiveGroup.CanRollbackFrames;

		public void SaveFrame()
		{
			_massiveGroup.SaveFrame();
			_cyclicFrameCounter.SaveFrame();

			var currentFrame = _cyclicFrameCounter.CurrentFrame;

			_syncedByFrames[currentFrame] = IsSynced;
		}

		public void Rollback(int frames)
		{
			_massiveGroup.Rollback(frames);
			_cyclicFrameCounter.Rollback(frames);

			var rollbackFrame = _cyclicFrameCounter.CurrentFrame;

			IsSynced = _syncedByFrames[rollbackFrame];
		}
	}
}