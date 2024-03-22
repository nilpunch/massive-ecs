using System;
using System.Runtime.CompilerServices;
using Unity.IL2CPP.CompilerServices;

namespace Massive
{
	[Il2CppSetOption(Option.NullChecks, false)]
	[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
	public class MassiveIdentifiers : Identifiers, IMassive
	{
		private readonly int[] _denseByFrames;
		private readonly int[] _sparseByFrames;
		private readonly int[] _maxIdByFrames;
		private readonly int[] _aliveCountByFrames;

		private readonly int _framesCapacity;
		private int _currentFrame;
		private int _savedFrames;

		public MassiveIdentifiers(int dataCapacity = Constants.DataCapacity, int framesCapacity = Constants.FramesCapacity)
			: base(dataCapacity)
		{
			_framesCapacity = framesCapacity;

			_denseByFrames = new int[framesCapacity * Dense.Length];
			_sparseByFrames = new int[framesCapacity * Sparse.Length];
			_maxIdByFrames = new int[framesCapacity];
			_aliveCountByFrames = new int[framesCapacity];
		}

		public int CanRollbackFrames => _savedFrames - 1;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void SaveFrame()
		{
			int currentAliveCount = AliveCount;
			int currentMaxId = MaxId;
			int nextFrame = Loop(_currentFrame + 1, _framesCapacity);

			// Copy everything from current state to next frame
			Array.Copy(Dense, 0, _denseByFrames, nextFrame * Dense.Length, currentMaxId);
			Array.Copy(Sparse, 0, _sparseByFrames, nextFrame * Sparse.Length, currentMaxId);
			_aliveCountByFrames[nextFrame] = currentAliveCount;
			_maxIdByFrames[nextFrame] = currentMaxId;

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
			int rollbackAliveCount = _aliveCountByFrames[_currentFrame];
			int rollbackMaxId = _maxIdByFrames[_currentFrame];
			int rollbackFrame = _currentFrame;

			Array.Copy(_denseByFrames, rollbackFrame * Dense.Length, Dense, 0, rollbackMaxId);
			Array.Copy(_sparseByFrames, rollbackFrame * Sparse.Length, Sparse, 0, rollbackMaxId);
			AliveCount = rollbackAliveCount;
			MaxId = rollbackMaxId;
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