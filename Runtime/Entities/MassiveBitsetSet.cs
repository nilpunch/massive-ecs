using System;
using System.Runtime.CompilerServices;

namespace Massive
{
	public class MassiveBitsetSet : BitsetSet, IMassive
	{
		private readonly CyclicFrameCounter _cyclicFrameCounter;

		private readonly short[][] _dataByFrames;

		public MassiveBitsetSet(int bitsPerElement = Constants.DefaultBitsPerElement, int bitsPerBitset = Constants.DefaultBitsPerBitset, int capacity = Constants.DefaultCapacity, int framesCapacity = Constants.DefaultFramesCapacity)
			: base(bitsPerElement, bitsPerBitset, capacity)
		{
			_cyclicFrameCounter = new CyclicFrameCounter(framesCapacity);

			_dataByFrames = new short[framesCapacity][];

			for (int i = 0; i < framesCapacity; i++)
			{
				_dataByFrames[i] = new short[Data.Length];
			}
		}

		public int CanRollbackFrames => _cyclicFrameCounter.CanRollbackFrames;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void SaveFrame()
		{
			_cyclicFrameCounter.SaveFrame();

			int currentFrame = _cyclicFrameCounter.CurrentFrame;

			// Copy everything from current state to current frame
			Array.Copy(Data, _dataByFrames[currentFrame], Data.Length);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Rollback(int frames)
		{
			_cyclicFrameCounter.Rollback(frames);

			// Copy everything from rollback frame to current state
			int rollbackFrame = _cyclicFrameCounter.CurrentFrame;

			Array.Copy(_dataByFrames[rollbackFrame], Data, Data.Length);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override void Resize(int capacity)
		{
			base.Resize(capacity);

			for (int i = 0; i < _cyclicFrameCounter.FramesCapacity; i++)
			{
				Array.Resize(ref _dataByFrames[i], Data.Length);
			}
		}
	}
}
