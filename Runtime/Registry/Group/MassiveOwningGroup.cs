namespace Massive
{
	public class MassiveOwningGroup : OwningGroup, IMassive
	{
		private readonly CyclicFrameCounter _cyclicFrameCounter;

		private readonly int[] _lengthByFrames;
		private readonly bool[] _syncedByFrames;

		public MassiveOwningGroup(ISet[] owned, IReadOnlySet[] other = null, IFilter filter = null, int framesCapacity = Constants.FramesCapacity)
			: base(owned, other, filter)
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

			_lengthByFrames[currentFrame] = GroupLength;
			_syncedByFrames[currentFrame] = IsSynced;
		}

		public void Rollback(int frames)
		{
			_cyclicFrameCounter.Rollback(frames);

			var rollbackFrame = _cyclicFrameCounter.CurrentFrame;

			GroupLength = _lengthByFrames[rollbackFrame];
			IsSynced = _syncedByFrames[rollbackFrame];
		}
	}
}