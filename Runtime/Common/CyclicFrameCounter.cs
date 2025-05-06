using System;
using System.Runtime.CompilerServices;

namespace Massive
{
	public class CyclicFrameCounter
	{
		private int _savedFrames;

		public CyclicFrameCounter(int framesCapacity = Constants.DefaultFramesCapacity)
		{
			FramesCapacity = framesCapacity;
		}

		public int FramesCapacity { get; }

		public int CurrentFrame { get; private set; }

		public int CanRollbackFrames
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => _savedFrames - 1;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void SaveFrame()
		{
			CurrentFrame = Loop(CurrentFrame + 1, FramesCapacity);
			_savedFrames = MathUtils.Min(_savedFrames + 1, FramesCapacity);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Rollback(int frames)
		{
			if (frames > CanRollbackFrames)
			{
				throw new ArgumentOutOfRangeException(nameof(frames), frames, $"Can't rollback this far. CanRollbackFrames: {CanRollbackFrames}.");
			}

			_savedFrames -= frames;
			CurrentFrame = LoopNegative(CurrentFrame - frames, FramesCapacity);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static int Loop(int a, int b)
		{
			return a % b;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static int LoopNegative(int a, int b)
		{
			var result = a % b;

			if (result < 0)
			{
				return result + b;
			}

			return result;
		}
	}
}
