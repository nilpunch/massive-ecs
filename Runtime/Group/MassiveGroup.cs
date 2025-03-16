using Unity.IL2CPP.CompilerServices;

namespace Massive
{
	[Il2CppSetOption(Option.NullChecks, false)]
	[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
	[Il2CppSetOption(Option.DivideByZeroChecks, false)]
	public sealed class MassiveGroup : Group, IMassive
	{
		private readonly IMassive _massiveMainSet;
		private readonly CyclicFrameCounter _cyclicFrameCounter;

		private readonly bool[] _syncedByFrames;

		public MassiveGroup(SparseSet[] included = null, SparseSet[] excluded = null, int framesCapacity = Constants.DefaultFramesCapacity, Entities entities = null)
			: base(new MassiveSparseSet(framesCapacity), included, excluded, entities)
		{
			// Fetch instance from base.
			_massiveMainSet = (IMassive)Set;

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
		}
	}
}
