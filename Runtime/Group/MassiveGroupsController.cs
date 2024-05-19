using System;

namespace Massive
{
	public class MassiveGroupsController : GroupsController, IMassive
	{
		private readonly CyclicFrameCounter _cyclicFrameCounter;

		public MassiveGroupsController(int nonOwningSetCapacity = Constants.DefaultSetCapacity, int framesCapacity = Constants.DefaultFramesCapacity)
			: base(new MassiveGroupFactory(nonOwningSetCapacity, framesCapacity))
		{
			_cyclicFrameCounter = new CyclicFrameCounter(framesCapacity);
		}

		public int CanRollbackFrames => _cyclicFrameCounter.CanRollbackFrames;

		public void SaveFrame()
		{
			_cyclicFrameCounter.SaveFrame();

			foreach (var group in AllGroups)
			{
				if (group is IMassive massive)
				{
					massive.SaveFrame();
				}
			}
		}

		public void Rollback(int frames)
		{
			_cyclicFrameCounter.Rollback(frames);

			foreach (var group in AllGroups)
			{
				if (group is IMassive massive)
				{
					massive.Rollback(Math.Min(frames, massive.CanRollbackFrames));
				}
			}
		}
	}
}
