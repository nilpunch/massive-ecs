using System.Collections.Generic;
using Unity.IL2CPP.CompilerServices;

namespace Massive
{
	[Il2CppSetOption(Option.NullChecks | Option.ArrayBoundsChecks, false)]
	public class MassiveNonOwningGroup : NonOwningGroup, IMassive
	{
		private readonly IMassive _massiveGroup;
		private readonly CyclicFrameCounter _cyclicFrameCounter;

		private readonly bool[] _syncedByFrames;

		public MassiveNonOwningGroup(IReadOnlyList<SparseSet> include, IReadOnlyList<SparseSet> exclude = null,
			int setCapacity = Constants.DefaultCapacity, int framesCapacity = Constants.DefaultFramesCapacity)
			: base(new MassiveSparseSet(setCapacity, framesCapacity), include, exclude)
		{
			// Fetch instance from base
			_massiveGroup = (IMassive)MainSet;

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
