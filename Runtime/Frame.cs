using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Massive
{
	[Unity.IL2CPP.CompilerServices.Il2CppSetOption(Unity.IL2CPP.CompilerServices.Option.NullChecks, false)]
	[Unity.IL2CPP.CompilerServices.Il2CppSetOption(Unity.IL2CPP.CompilerServices.Option.ArrayBoundsChecks, false)]
	[Unity.IL2CPP.CompilerServices.Il2CppSetOption(Unity.IL2CPP.CompilerServices.Option.DivideByZeroChecks, false)]
	public readonly unsafe ref struct Frame<TState>
	{
		private readonly Span<int> _sparse;
		private readonly Span<int> _dense;
		private readonly Span<TState> _data;
		private readonly int* _aliveCount;
		private readonly int* _maxId;
		private readonly int* _currentFrame;
		private readonly int _thisFrame;
		private readonly int _statesCapacity;

		public Frame(Span<int> sparse, Span<int> dense, Span<TState> data, int* aliveCount, int* maxId, int* currentFrame)
		{
			_sparse = sparse;
			_dense = dense;
			_data = data;
			_aliveCount = aliveCount;
			_maxId = maxId;
			_currentFrame = currentFrame;
			_thisFrame = *currentFrame;
			_statesCapacity = sparse.Length;
		}

		public int AliveCount => *_aliveCount;

		public int Create(TState state = default)
		{
			ThrowIfFrameIsNotCurrent();

			int count = *_aliveCount;
			int maxId = *_maxId;

			if (count == _statesCapacity)
			{
				throw new InvalidOperationException($"Exceeded limit of states per frame! Limit: {_statesCapacity}.");
			}

			_data[count] = state;

			// If there are unused elements in the dense array, return last
			if (count < maxId)
			{
				*_aliveCount += 1;
				return _dense[count];
			}

			*_aliveCount += 1;
			*_maxId += 1;

			_dense[count] = maxId;
			_sparse[maxId] = count;
			return maxId;
		}

		public void Delete(int id)
		{
			ThrowIfFrameIsNotCurrent();

			int aliveCount = *_aliveCount;
			int denseIndex = _sparse[id];

			if (denseIndex >= aliveCount)
			{
				throw new InvalidOperationException($"Id is not alive! Id: {id}.");
			}

			// If dense is the last used element, simply decrease count
			if (denseIndex == aliveCount - 1)
			{
				*_aliveCount -= 1;
				return;
			}

			*_aliveCount -= 1;

			int swapDenseIndex = aliveCount - 1;
			int swapId = _dense[swapDenseIndex];

			_data[denseIndex] = _data[swapDenseIndex];
			_dense[denseIndex] = swapId;
			_sparse[id] = swapDenseIndex;
			_dense[swapDenseIndex] = id;
			_sparse[swapId] = denseIndex;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public ref TState Get(int id)
		{
			ThrowIfFrameIsNotCurrent();

			if (!IsAlive(id))
			{
				throw new InvalidOperationException($"State does not exist! RequestedState: {id}.");
			}

			int denseIndex = _sparse[id];

			return ref _data[denseIndex];
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Span<TState> GetAllStates()
		{
			ThrowIfFrameIsNotCurrent();

			return _data.Slice(0, *_aliveCount);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Span<int> GetAllIds()
		{
			ThrowIfFrameIsNotCurrent();

			return _dense.Slice(0, *_aliveCount);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool IsAlive(int id)
		{
			ThrowIfFrameIsNotCurrent();

			if (id >= _statesCapacity)
				return false;

			int denseIndex = _sparse[id];

			return denseIndex < *_aliveCount && _dense[denseIndex] == id;
		}

		[Conditional("UNITY_EDITOR")]
		private void ThrowIfFrameIsNotCurrent()
		{
			if (_thisFrame != *_currentFrame)
			{
				throw new InvalidOperationException($"This frame is not valid! ThisFrame: {_thisFrame}, CurrentFrame {*_currentFrame}.");
			}
		}
	}
}