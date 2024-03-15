using System;
using System.Runtime.CompilerServices;
using Unity.IL2CPP.CompilerServices;

namespace Massive
{
	[Il2CppSetOption(Option.NullChecks, false)]
	[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
	[Il2CppSetOption(Option.DivideByZeroChecks, false)]
	public class MassiveIdentifiers : Identifiers, IMassive
	{
		private readonly int[] _idsByFrames;
		private readonly int[] _maxIdByFrames;
		private readonly int[] _availableByFrames;
		private readonly int[] _nextByFrames;

		private readonly int _framesCapacity;
		private int _currentFrame;
		private int _savedFrames;

		public MassiveIdentifiers(int dataCapacity = Constants.DataCapacity, int framesCapacity = Constants.FramesCapacity)
			: base(dataCapacity)
		{
			_framesCapacity = framesCapacity;

			_idsByFrames = new int[framesCapacity * Ids.Length];
			_maxIdByFrames = new int[framesCapacity];
			_availableByFrames = new int[framesCapacity];
			_nextByFrames = new int[framesCapacity];
		}

		public int CanRollbackFrames => _savedFrames - 1;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void SaveFrame()
		{
			int currentMaxId = MaxId;
			int currentAvailable = Available;
			int currentNext = Next;
			int nextFrame = Loop(_currentFrame + 1, _framesCapacity);

			// Copy everything from current state to next frame
			Array.Copy(Ids, 0, _idsByFrames, nextFrame * Ids.Length, currentMaxId);
			_maxIdByFrames[nextFrame] = currentMaxId;
			_availableByFrames[nextFrame] = currentAvailable;
			_nextByFrames[nextFrame] = currentNext;

			_currentFrame = nextFrame;
			_savedFrames = Math.Min(_savedFrames + 1, _framesCapacity);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Rollback(int frames)
		{
			if (frames > CanRollbackFrames)
			{
				throw new ArgumentOutOfRangeException(nameof(frames), frames, $"Can't rollback this far. CanRollbackFrames: {CanRollbackFrames}.");
			}

			_savedFrames -= frames;
			_currentFrame = LoopNegative(_currentFrame - frames, _framesCapacity);

			// Copy everything from rollback frame to current state
			int rollbackMaxId = _maxIdByFrames[_currentFrame];
			int rollbackAvailable = _availableByFrames[_currentFrame];
			int rollbackNext = _nextByFrames[_currentFrame];
			int rollbackFrame = _currentFrame;

			Array.Copy(_idsByFrames, rollbackFrame * Ids.Length, Ids, 0, rollbackMaxId);
			MaxId = rollbackMaxId;
			Available = rollbackAvailable;
			Next = rollbackNext;
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