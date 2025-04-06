using System;
using System.Runtime.CompilerServices;

namespace Massive
{
	/// <summary>
	/// Rollback extension for <see cref="World"/>.
	/// </summary>
	public class MassiveWorld : World, IMassive
	{
		private readonly CyclicFrameCounter _cyclicFrameCounter;

		private readonly World[] _frames;

		public MassiveWorld()
			: this(new MassiveWorldConfig())
		{
		}

		public MassiveWorld(MassiveWorldConfig worldConfig)
			: base(worldConfig)
		{
			_cyclicFrameCounter = new CyclicFrameCounter(worldConfig.FramesCapacity);

			_frames = new World[worldConfig.FramesCapacity];

			for (var i = 0; i < worldConfig.FramesCapacity; i++)
			{
				_frames[i] = new World(worldConfig);
			}
		}

		public event Action FrameSaved;
		public event Action<int> Rollbacked;

		public int CanRollbackFrames => _cyclicFrameCounter.CanRollbackFrames;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void SaveFrame()
		{
			_cyclicFrameCounter.SaveFrame();

			var currentFrame = _cyclicFrameCounter.CurrentFrame;

			this.CopyTo(_frames[currentFrame]);

			FrameSaved?.Invoke();
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Rollback(int frames)
		{
			_cyclicFrameCounter.Rollback(frames);

			var rollbackFrame = _cyclicFrameCounter.CurrentFrame;

			_frames[rollbackFrame].CopyTo(this);

			Rollbacked?.Invoke(frames);
		}
	}
}
