using System.Collections.Generic;
using Unity.IL2CPP.CompilerServices;

namespace Massive
{
	[Il2CppSetOption(Option.NullChecks | Option.ArrayBoundsChecks, false)]
	public class MassiveNonOwningGroup : NonOwningGroup, IMassive
	{
		private readonly IMassive _massiveMainSet;
		private readonly CyclicFrameCounter _cyclicFrameCounter;

		private readonly bool[] _syncedByFrames;

		public MassiveNonOwningGroup(IReadOnlyList<SparseSet> included, IReadOnlyList<SparseSet> excluded = null, int framesCapacity = Constants.DefaultFramesCapacity, Entities entities = null)
			: base(new MassiveSparseSet(framesCapacity), included, excluded, entities)
		{
			// Fetch instance from base
			_massiveMainSet = (IMassive)MainSet;

			_cyclicFrameCounter = new CyclicFrameCounter(framesCapacity);

			_syncedByFrames = new bool[framesCapacity];
		}

		public int CanRollbackFrames => _massiveMainSet.CanRollbackFrames;

		public void SaveFrame()
		{
			_massiveMainSet.SaveFrame();
			_cyclicFrameCounter.SaveFrame();

			var currentFrame = _cyclicFrameCounter.CurrentFrame;

			_syncedByFrames[currentFrame] = IsSynced;
		}

		public void Rollback(int frames)
		{
			_massiveMainSet.Rollback(frames);
			_cyclicFrameCounter.Rollback(frames);

			var rollbackFrame = _cyclicFrameCounter.CurrentFrame;

			IsSynced = _syncedByFrames[rollbackFrame];
			SyncCount();
		}
	}
}
