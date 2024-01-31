using System;
using System.Runtime.CompilerServices;

namespace Massive
{
	[Unity.IL2CPP.CompilerServices.Il2CppSetOption(Unity.IL2CPP.CompilerServices.Option.NullChecks, false)]
	[Unity.IL2CPP.CompilerServices.Il2CppSetOption(Unity.IL2CPP.CompilerServices.Option.ArrayBoundsChecks, false)]
	[Unity.IL2CPP.CompilerServices.Il2CppSetOption(Unity.IL2CPP.CompilerServices.Option.DivideByZeroChecks, false)]
	public class MassiveData<T> : IMassiveData where T : struct
	{
		private readonly int _framesCapacity;
		private readonly int _dataCapacity;
		private readonly T[] _dataByFrames;
		private readonly int[] _denseByFrames;
		private readonly int[] _sparseByFrames;
		private readonly int[] _maxIdByFrames;
		private readonly int[] _aliveCountByFrames;

		private int _currentFrame;
		private int _framesCount;

		public MassiveData(int framesCapacity = 120, int dataCapacity = 100)
		{
			// Reserve 2 frames
			// One for rollback restoration, another one for current frame
			_framesCapacity = framesCapacity + 2;

			_dataCapacity = dataCapacity;
			_dataByFrames = new T[_framesCapacity * dataCapacity];
			_denseByFrames = new int[_framesCapacity * dataCapacity];
			_sparseByFrames = new int[_framesCapacity * dataCapacity];
			_maxIdByFrames = new int[_framesCapacity];
			_aliveCountByFrames = new int[_framesCapacity];
		}

		public unsafe Frame<T> CurrentFrame
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				int startIndex = _currentFrame * _dataCapacity;

				fixed (int* aliveCount = &_aliveCountByFrames[_currentFrame])
				fixed (int* maxId = &_maxIdByFrames[_currentFrame])
				fixed (int* currentFrame = &_currentFrame)
					return new Frame<T>(
						new Span<int>(_sparseByFrames, startIndex, _dataCapacity),
						new Span<int>(_denseByFrames, startIndex, _dataCapacity),
						new Span<T>(_dataByFrames, startIndex, _dataCapacity),
						aliveCount, maxId, currentFrame);
			}
		}

		public int CanRollbackFrames => _framesCount - 1;

		public void SaveFrame()
		{
			int nextFrame = Loop(_currentFrame + 1, _framesCapacity);
			int currentAliveCount = _aliveCountByFrames[_currentFrame];
			int currentMaxId = _maxIdByFrames[_currentFrame];

			int currentFrameIndex = _currentFrame * _dataCapacity;
			int nextFrameIndex = nextFrame * _dataCapacity;

			Array.Copy(_dataByFrames, currentFrameIndex, _dataByFrames, nextFrameIndex, currentAliveCount);
			Array.Copy(_denseByFrames, currentFrameIndex, _denseByFrames, nextFrameIndex, currentMaxId);
			Array.Copy(_sparseByFrames, currentFrameIndex, _sparseByFrames, nextFrameIndex, currentMaxId);

			_currentFrame = nextFrame;
			_aliveCountByFrames[nextFrame] = currentAliveCount;
			_maxIdByFrames[nextFrame] = currentMaxId;

			// Limit count by framesCapacity-1, because one frame is current and so not counted
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
		public int Create(T data = default)
		{
			return CurrentFrame.Create(data);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Delete(int id)
		{
			CurrentFrame.Delete(id);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public ref T Get(int id)
		{
			return ref CurrentFrame.Get(id);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Span<T> GetAll()
		{
			return CurrentFrame.GetAll();
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Span<int> GetAllIds()
		{
			return CurrentFrame.GetAllIds();
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