using System.Runtime.CompilerServices;

namespace Massive
{
	public class MassiveManagedVariable<T> : IMassive where T : IManaged<T>
	{
		private readonly CyclicFrameCounter _cyclicFrameCounter;
		private readonly T[] _valueByFrames;
		private T _value;

		public ref T Value
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => ref _value;
		}

		public MassiveManagedVariable(T initialValue = default, int framesCapacity = Constants.DefaultFramesCapacity)
		{
			_cyclicFrameCounter = new CyclicFrameCounter(framesCapacity);
			_valueByFrames = new T[framesCapacity];
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
