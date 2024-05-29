using System;

namespace Massive
{
	public class MassiveGroupRegistry : GroupRegistry, IMassive
	{
		private readonly CyclicFrameCounter _cyclicFrameCounter;

		public MassiveGroupRegistry(int nonOwningSetCapacity = Constants.DefaultSetCapacity, int framesCapacity = Constants.DefaultFramesCapacity)
			: base(new MassiveGroupFactory(nonOwningSetCapacity, framesCapacity))
		{
			_cyclicFrameCounter = new CyclicFrameCounter(framesCapacity);
		}

		public int CanRollbackFrames => _cyclicFrameCounter.CanRollbackFrames;

		public void SaveFrame()
		{
			_cyclicFrameCounter.SaveFrame();

			for (var i = 0; i < All.Count; i++)
			{
				if (All[i] is IMassive massive)
				{
					massive.SaveFrame();
				}
			}
		}

		public void Rollback(int frames)
		{
			_cyclicFrameCounter.Rollback(frames);

			for (var i = 0; i < All.Count; i++)
			{
				if (All[i] is IMassive massive)
				{
					massive.Rollback(Math.Min(frames, massive.CanRollbackFrames));
				}
			}
		}
	}
}
