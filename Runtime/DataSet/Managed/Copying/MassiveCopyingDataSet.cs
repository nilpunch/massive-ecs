using System.Runtime.CompilerServices;
using Unity.IL2CPP.CompilerServices;

namespace Massive
{
	/// <summary>
	/// Rollback extension for <see cref="Massive.CopyingDataSet{T}"/>.
	/// Swaps data when elements are moved.
	/// Used for managed components to reduce allocations.
	/// </summary>
	[Il2CppSetOption(Option.NullChecks, false)]
	[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
	public class MassiveCopyingDataSet<T> : CopyingDataSet<T>, IMassive where T : ICopyable<T>
	{
		private readonly CyclicFrameCounter _cyclicFrameCounter;

		private readonly CopyingDataSet<T>[] _frames;

		public MassiveCopyingDataSet(int framesCapacity = Constants.DefaultFramesCapacity,
			int pageSize = Constants.DefaultPageSize, Packing packing = Packing.Continuous)
			: base(pageSize, packing)
		{
			_cyclicFrameCounter = new CyclicFrameCounter(framesCapacity);

			_frames = new CopyingDataSet<T>[framesCapacity];

			for (var i = 0; i < framesCapacity; i++)
			{
				_frames[i] = new CopyingDataSet<T>(pageSize, packing);
			}
		}

		public int CanRollbackFrames => _cyclicFrameCounter.CanRollbackFrames;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void SaveFrame()
		{
			_cyclicFrameCounter.SaveFrame();

			var currentFrame = _cyclicFrameCounter.CurrentFrame;

			CopyToCopyable(_frames[currentFrame]);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Rollback(int frames)
		{
			_cyclicFrameCounter.Rollback(frames);

			var rollbackFrame = _cyclicFrameCounter.CurrentFrame;

			_frames[rollbackFrame].CopyToCopyable(this);
		}
	}
}
