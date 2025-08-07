using System.Runtime.CompilerServices;
using Unity.IL2CPP.CompilerServices;

namespace Massive
{
	/// <summary>
	/// Rollback extension for <see cref="Entifiers"/>.
	/// </summary>
	[Il2CppSetOption(Option.NullChecks, false)]
	[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
	public class MassiveEntifiers : Entifiers, IMassive
	{
		private readonly CyclicFrameCounter _cyclicFrameCounter;

		private readonly Entifiers[] _frames;

		public MassiveEntifiers(int framesCapacity = Constants.DefaultFramesCapacity)
		{
			_cyclicFrameCounter = new CyclicFrameCounter(framesCapacity);

			_frames = new Entifiers[framesCapacity];

			for (var i = 0; i < framesCapacity; i++)
			{
				_frames[i] = new Entifiers();
			}
		}

		public int CanRollbackFrames => _cyclicFrameCounter.CanRollbackFrames;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void SaveFrame()
		{
			_cyclicFrameCounter.SaveFrame();

			var currentFrame = _cyclicFrameCounter.CurrentFrame;

			CopyTo(_frames[currentFrame]);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Rollback(int frames)
		{
			_cyclicFrameCounter.Rollback(frames);

			var rollbackFrame = _cyclicFrameCounter.CurrentFrame;

			_frames[rollbackFrame].CopyTo(this);
		}
	}
}
