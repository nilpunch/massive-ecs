using System;
using System.Collections.Generic;

namespace Massive
{
	public class MassiveGroupsController : GroupsController, IMassive
	{
		private readonly CyclicFrameCounter _cyclicFrameCounter;

		public MassiveGroupsController(int nonOwningDataCapacity = Constants.DataCapacity, int framesCapacity = Constants.FramesCapacity)
			: base(nonOwningDataCapacity)
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

		protected override IOwningGroup CreateOwningGroup(IReadOnlyList<ISet> owned,
			IReadOnlyList<IReadOnlySet> include = null, IReadOnlyList<IReadOnlySet> exclude = null)
		{
			var massiveOwningGroup = new MassiveOwningGroup(owned, include, exclude, _cyclicFrameCounter.FramesCapacity);
			massiveOwningGroup.SaveFrame(); // Save first empty frame so we can rollback to it
			return massiveOwningGroup;
		}

		protected override IGroup CreateNonOwningGroup(IReadOnlyList<IReadOnlySet> include,
			IReadOnlyList<IReadOnlySet> exclude = null, int dataCapacity = Constants.DataCapacity)
		{
			var massiveNonOwningGroup = new MassiveNonOwningGroup(include, exclude, dataCapacity, _cyclicFrameCounter.FramesCapacity);
			massiveNonOwningGroup.SaveFrame(); // Save first empty frame so we can rollback to it
			return massiveNonOwningGroup;
		}
	}
}
