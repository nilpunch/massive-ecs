using System.Runtime.CompilerServices;
using Unity.IL2CPP.CompilerServices;

namespace Massive
{
	/// <summary>
	/// Rollback extension for <see cref="Massive.SparseSet"/>.
	/// </summary>
	[Il2CppSetOption(Option.NullChecks, false)]
	[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
	public class MassiveSparseSet : SparseSet, IMassive
	{
		private readonly CyclicFrameCounter _cyclicFrameCounter;

		private readonly SparseSet[] _frames;

		public MassiveSparseSet(int framesCapacity = Constants.DefaultFramesCapacity, Packing packing = Packing.Continuous)
			: base(packing)
		{
			_cyclicFrameCounter = new CyclicFrameCounter(framesCapacity);

			_frames = new SparseSet[framesCapacity];

			for (var i = 0; i < framesCapacity; i++)
			{
				_frames[i] = new SparseSet(packing);
			}
		}

		public int CanRollbackFrames => _cyclicFrameCounter.CanRollbackFrames;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void SaveFrame()
		{
			_cyclicFrameCounter.SaveFrame();

			var currentFrame = _cyclicFrameCounter.CurrentFrame;

			CopySparseTo(_frames[currentFrame]);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Rollback(int frames)
		{
			_cyclicFrameCounter.Rollback(frames);

			var rollbackFrame = _cyclicFrameCounter.CurrentFrame;

			_frames[rollbackFrame].CopySparseTo(this);
		}
	}
}
