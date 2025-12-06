using System.Runtime.CompilerServices;

namespace Massive
{
	public class MassiveSystems : Systems, IMassive
	{
		private readonly CyclicFrameCounter _cyclicFrameCounter;

		private readonly Allocator[] _frames;

		public MassiveSystems(int framesCapacity)
		{
			_cyclicFrameCounter = new CyclicFrameCounter(framesCapacity);

			_frames = new Allocator[framesCapacity];

			for (var i = 0; i < framesCapacity; i++)
			{
				_frames[i] = new Allocator();
			}
		}

		public int CanRollbackFrames => _cyclicFrameCounter.CanRollbackFrames;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void SaveFrame()
		{
			_cyclicFrameCounter.SaveFrame();

			var currentFrame = _cyclicFrameCounter.CurrentFrame;

			Allocator.CopyTo(_frames[currentFrame]);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Rollback(int frames)
		{
			_cyclicFrameCounter.Rollback(frames);

			var rollbackFrame = _cyclicFrameCounter.CurrentFrame;

			_frames[rollbackFrame].CopyTo(Allocator);
		}
	}
}
