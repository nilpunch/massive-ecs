using System;
using System.Runtime.CompilerServices;
using Unity.IL2CPP.CompilerServices;

namespace Massive
{
	[Il2CppSetOption(Option.DivideByZeroChecks, false)]
	public class CyclicFrameCounter
	{
		private int _savedFrames;

		public CyclicFrameCounter(int framesCapacity = Constants.FramesCapacity)
		{
			FramesCapacity = framesCapacity;
		}

		public int FramesCapacity { get; }

		public int CurrentFrame { get; private set; }

		public int CanRollbackFrames => _savedFrames - 1;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void SaveFrame()
		{
			CurrentFrame = Loop(CurrentFrame + 1, FramesCapacity);
			_savedFrames = Math.Min(_savedFrames + 1, FramesCapacity);
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
			int result = a % b;

			if (result < 0)
			{
				return result + b;
			}

			return result;
		}
	}
}
