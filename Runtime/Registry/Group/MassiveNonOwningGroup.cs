namespace Massive
{
	public class MassiveNonOwningGroup : NonOwningGroup, IMassive
	{
		private readonly IMassive _massiveGroup;
		private readonly CyclicFrameCounter _cyclicFrameCounter;

		private readonly bool[] _syncedByFrames;

		public MassiveNonOwningGroup(ISet[] other, IFilter filter = null, int dataCapacity = Constants.DataCapacity,
			int framesCapacity = Constants.FramesCapacity)
			: base(other, new MassiveSparseSet(dataCapacity, framesCapacity), filter)
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

			_syncedByFrames[_cyclicFrameCounter.CurrentFrame] = IsSynced;
		}

		public void Rollback(int frames)
		{
			_massiveGroup.Rollback(frames);
			_cyclicFrameCounter.Rollback(frames);

			IsSynced = _syncedByFrames[_cyclicFrameCounter.CurrentFrame];
		}
	}
}