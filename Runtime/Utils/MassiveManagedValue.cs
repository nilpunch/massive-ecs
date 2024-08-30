using System.Runtime.CompilerServices;

namespace Massive
{
	public class MassiveManagedValue<TValue> : IMassive where TValue : IManaged<TValue>
	{
		private readonly CyclicFrameCounter _cyclicFrameCounter;
		private readonly TValue[] _valueByFrames;
		private TValue _value;

		public ref TValue Value
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => ref _value;
		}

		public MassiveManagedValue(TValue initialValue = default, int framesCapacity = Constants.DefaultFramesCapacity)
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

			Value.CopyTo(ref _valueByFrames[_cyclicFrameCounter.CurrentFrame]);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Rollback(int frames)
		{
			_cyclicFrameCounter.Rollback(frames);

			_valueByFrames[_cyclicFrameCounter.CurrentFrame].CopyTo(ref Value);
		}
	}
}
