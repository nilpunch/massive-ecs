using System;
using System.Runtime.CompilerServices;

namespace Massive
{
	[Unity.IL2CPP.CompilerServices.Il2CppSetOption(Unity.IL2CPP.CompilerServices.Option.NullChecks, false)]
	[Unity.IL2CPP.CompilerServices.Il2CppSetOption(Unity.IL2CPP.CompilerServices.Option.ArrayBoundsChecks, false)]
	[Unity.IL2CPP.CompilerServices.Il2CppSetOption(Unity.IL2CPP.CompilerServices.Option.DivideByZeroChecks, false)]
	public class WorldState<TState> : IWorldState where TState : struct
	{
		private readonly int _framesCapacity;
		private readonly int _statesCapacity;
		private readonly TState[] _dataByFrames;
		private readonly int[] _denseByFrames;
		private readonly int[] _sparseByFrames;
		private readonly int[] _framesAliveCount;

		private int _currentFrame;
		private int _framesCount;

		public WorldState(int frames = 120, int statesCapacity = 100)
		{
			// Reserve 2 frames
			// One for rollback restoration, another one for current frame
			_framesCapacity = frames + 2;

			_statesCapacity = statesCapacity;
			_dataByFrames = new TState[_framesCapacity * statesCapacity];
			_denseByFrames = new int[_framesCapacity * statesCapacity];
			_sparseByFrames = new int[_framesCapacity * statesCapacity];
			_framesAliveCount = new int[_framesCapacity];
		}

		public unsafe Frame<TState> CurrentFrame
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				int startIndex = _currentFrame * _statesCapacity;

				fixed (int* aliveCount = &_framesAliveCount[_currentFrame])
				fixed (int* currentFrame = &_currentFrame)
					return new Frame<TState>(
						new Span<int>(_sparseByFrames, startIndex, _statesCapacity),
						new Span<int>(_denseByFrames, startIndex, _statesCapacity),
						new Span<TState>(_dataByFrames, startIndex, _statesCapacity),
						aliveCount,
						currentFrame);
			}
		}

		public void SaveFrame()
		{
			int nextFrame = Loop(_currentFrame + 1, _framesCapacity);
			int currentAliveCount = _framesAliveCount[_currentFrame];

			int currentFrameIndex = _currentFrame * _statesCapacity;
			int nextFrameIndex = nextFrame * _statesCapacity;

			if (currentAliveCount > 0)
			{
				Array.Copy(_dataByFrames, currentFrameIndex, _dataByFrames, nextFrameIndex, currentAliveCount);
				Array.Copy(_denseByFrames, currentFrameIndex, _denseByFrames, nextFrameIndex, currentAliveCount);
			}

			Array.Copy(_sparseByFrames, currentFrameIndex, _sparseByFrames, nextFrameIndex, _statesCapacity);

			_currentFrame = nextFrame;
			_framesAliveCount[nextFrame] = currentAliveCount;

			// Limit count by maxFrames-1, because one frame is current and so not counted
			_framesCount = Math.Min(_framesCount + 1, _framesCapacity - 1);
		}

		public void Rollback(int frames)
		{
			// One frame is reserved for restoring
			int canRollback = _framesCount - 1;

			if (frames > canRollback)
			{
				throw new InvalidOperationException($"Can't rollback this far. CanRollback:{canRollback}, Requested: {frames}.");
			}

			// Add one frame to the rollback to appear at one frame before the target frame
			frames += 1;

			_framesCount -= frames;
			_currentFrame = LoopNegative(_currentFrame - frames, _framesCapacity);

			// Populate target frame with data from rollback frame
			// This will keep rollback frame untouched
			SaveFrame();
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Create(int id, TState state = default)
		{
			CurrentFrame.Create(id, state);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Delete(int id)
		{
			CurrentFrame.Delete(id);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public ref TState Get(int id)
		{
			return ref CurrentFrame.Get(id);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Span<TState> GetAll()
		{
			return CurrentFrame.GetAll();
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool IsAlive(int id)
		{
			return CurrentFrame.IsAlive(id);
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