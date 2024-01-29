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
		private readonly int* _currentFrame;
		private readonly int _thisFrame;
		private readonly int _statesCapacity;

		public Frame(Span<int> sparse, Span<int> dense, Span<TState> data, int* aliveCount, int* currentFrame)
		{
			_sparse = sparse;
			_dense = dense;
			_data = data;
			_aliveCount = aliveCount;
			_currentFrame = currentFrame;
			_thisFrame = *currentFrame;
			_statesCapacity = sparse.Length;
		}

		public int AliveCount => *_aliveCount;

		public void Create(int id, TState state = default)
		{
			ThrowIfFrameIsNotCurrent();

			int nextDenseIndex = *_aliveCount;

			if (nextDenseIndex == _statesCapacity)
			{
				throw new InvalidOperationException($"Exceeded limit of states per frame! Limit: {_statesCapacity}.");
			}

			_data[nextDenseIndex] = state;
			_dense[nextDenseIndex] = id;
			_sparse[id] = nextDenseIndex;

			*_aliveCount += 1;
		}

		public void Delete(int id)
		{
			ThrowIfFrameIsNotCurrent();

			int aliveCount = *_aliveCount;
			int denseIndex = _sparse[id];

			if (denseIndex >= aliveCount)
			{
				throw new InvalidOperationException($"Index is not alive! SparseIndex: {id}.");
			}

			int swapDenseIndex = aliveCount - 1;
			int swapSparseIndex = _dense[swapDenseIndex];
			TState swapData = _data[swapDenseIndex];

			_data[denseIndex] = swapData;
			_dense[denseIndex] = swapSparseIndex;
			_sparse[swapSparseIndex] = denseIndex;

			*_aliveCount -= 1;
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