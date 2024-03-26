namespace Massive
{
	public class MassiveNonOwningGroup : NonOwningGroup, IMassive
	{
		private readonly IMassive _massiveGroup;

		private int _currentFrame;

		public MassiveNonOwningGroup(ISet[] other, IFilter filter = null, int dataCapacity = Constants.DataCapacity, int framesCapacity = Constants.FramesCapacity)
			: base(other, new MassiveSparseSet(dataCapacity, framesCapacity), filter)
		{
			// Fetch instance from base
			_massiveGroup = (IMassive)Group;
		}

		public int CanRollbackFrames => _massiveGroup.CanRollbackFrames;

		public void SaveFrame()
		{
			_massiveGroup.SaveFrame();
		}

		public void Rollback(int frames)
		{
			_massiveGroup.Rollback(frames);

			if (CanRollbackFrames == 0)
			{
				IsWaken = false;
			}
		}
	}
}