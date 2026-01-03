using System;
using System.Runtime.CompilerServices;

namespace Massive
{
	public class CyclicFrameCounter : IMassive
	{
		private int _savedFrames;

		public CyclicFrameCounter(int framesCapacity = Constants.DefaultFramesCapacity)
		{
			FramesCapacity = framesCapacity;
		}

		/// <summary>
		/// The maximum number of frames that can be saved.
		/// </summary>
		public int FramesCapacity { get; }

		/// <summary>
		/// The index of the current frame.
		/// </summary>
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
			NegativeArgumentException.ThrowIfNegative(frames);

			if (frames > CanRollbackFrames)
			{
				throw new ArgumentOutOfRangeException(nameof(frames), frames, $"Can't rollback this far. CanRollbackFrames: {CanRollbackFrames}.");
			}

			_savedFrames -= frames;
			CurrentFrame = LoopNegative(CurrentFrame - frames, FramesCapacity);
		}

		/// <summary>
		/// Returns the frame index from a previous frame without modifying the current state.<br/>
		/// A value of 0 returns the <see cref="CurrentFrame"/>.
		/// </summary>
		/// <param name="frames">
		/// The number of frames to peek back. Must be non-negative and not exceed <see cref="CanRollbackFrames"/>.
		/// </param>
		/// <returns>
		/// The cycled frame index corresponding to the specified number of frames ago.
		/// </returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public int Peekback(int frames)
		{
			NegativeArgumentException.ThrowIfNegative(frames);

			if (frames > CanRollbackFrames)
			{
				throw new ArgumentOutOfRangeException(nameof(frames), frames, $"Can't peekback this far. CanRollbackFrames: {CanRollbackFrames}.");
			}

			return LoopNegative(CurrentFrame - frames, FramesCapacity);
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
