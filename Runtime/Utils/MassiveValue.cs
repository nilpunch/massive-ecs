using System.Runtime.CompilerServices;

namespace Massive
{
	public class MassiveValue<TValue> : IMassive
	{
		private readonly CyclicFrameCounter _cyclicFrameCounter;
		private readonly TValue[] _valueByFrames;

		public TValue Value { get; set; }

		public MassiveValue(TValue initialValue = default, int framesCapacity = Constants.DefaultFramesCapacity)
		{
			_cyclicFrameCounter = new CyclicFrameCounter(framesCapacity);
			_valueByFrames = new TValue[framesCapacity];
			Value = initialValue;
		}

		public int CanRollbackFrames => _cyclicFrameCounter.CanRollbackFrames;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void SaveFrame()
		{
			_cyclicFrameCounter.SaveFrame();

			_valueByFrames[_cyclicFrameCounter.CurrentFrame] = Value;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Rollback(int frames)
		{
			_cyclicFrameCounter.Rollback(frames);

			Value = _valueByFrames[_cyclicFrameCounter.CurrentFrame];
		}
	}
}
