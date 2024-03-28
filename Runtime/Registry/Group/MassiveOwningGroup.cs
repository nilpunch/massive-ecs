namespace Massive
{
	public class MassiveOwningGroup : OwningGroup, IMassive
	{
		private readonly CyclicFrameCounter _cyclicFrameCounter;

		private readonly int[] _lengthByFrames;
		private readonly bool[] _syncedByFrames;
		private readonly IGroup[] _extendedGroupByFrames;

		public MassiveOwningGroup(ISet[] owned, IReadOnlySet[] other = null, IFilter filter = null, int framesCapacity = Constants.FramesCapacity)
			: base(owned, other, filter)
		{
			_cyclicFrameCounter = new CyclicFrameCounter(framesCapacity);

			_lengthByFrames = new int[framesCapacity];
			_syncedByFrames = new bool[framesCapacity];
			_extendedGroupByFrames = new IGroup[framesCapacity];
		}

		public int CanRollbackFrames => _cyclicFrameCounter.CanRollbackFrames;

		public void SaveFrame()
		{
			_cyclicFrameCounter.SaveFrame();

			var currentFrame = _cyclicFrameCounter.CurrentFrame;

			_lengthByFrames[currentFrame] = GroupLength;
			_syncedByFrames[currentFrame] = IsSynced;
			_extendedGroupByFrames[currentFrame] = ExtendedGroup;
		}

		public void Rollback(int frames)
		{
			_cyclicFrameCounter.Rollback(frames);

			var rollbackFrame = _cyclicFrameCounter.CurrentFrame;

			GroupLength = _lengthByFrames[rollbackFrame];
			IsSynced = _syncedByFrames[rollbackFrame];
			ExtendedGroup = _extendedGroupByFrames[rollbackFrame];
		}
	}
}