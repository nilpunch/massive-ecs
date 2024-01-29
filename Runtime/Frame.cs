using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Massive
{
	[Unity.IL2CPP.CompilerServices.Il2CppSetOption(Unity.IL2CPP.CompilerServices.Option.NullChecks, false)]
	[Unity.IL2CPP.CompilerServices.Il2CppSetOption(Unity.IL2CPP.CompilerServices.Option.ArrayBoundsChecks, false)]
	[Unity.IL2CPP.CompilerServices.Il2CppSetOption(Unity.IL2CPP.CompilerServices.Option.DivideByZeroChecks, false)]
	public readonly unsafe ref struct Frame<TState> where TState : IState
	{
		private readonly Span<int> _sparse;
		private readonly Span<TState> _dense;
		private readonly int* _aliveCount;
		private readonly int* _currentFrame;
		private readonly int _thisFrame;
		private readonly int _statesCapacity;

		public Frame(Span<int> sparse, Span<TState> dense, int* aliveCount, int* currentFrame)
		{
			_sparse = sparse;
			_dense = dense;
			_aliveCount = aliveCount;
			_currentFrame = currentFrame;
			_thisFrame = *currentFrame;
			_statesCapacity = sparse.Length;
		}

		public int Create(TState state = default)
		{
			ThrowIfFrameIsNotCurrent();

			int nextSparseIndex = *_aliveCount;

			if (nextSparseIndex == _statesCapacity)
			{
				throw new InvalidOperationException($"Exceeded limit of states per frame! Limit: {_statesCapacity}.");
			}

			state.SparseIndex = nextSparseIndex;

			int denseIndex = _sparse[nextSparseIndex];
			_dense[denseIndex] = state;

			*_aliveCount += 1;
			return nextSparseIndex;
		}

		public void Delete(int sparseIndex)
		{
			ThrowIfFrameIsNotCurrent();

			int aliveCount = *_aliveCount;
			int denseIndex = _sparse[sparseIndex];

			if (denseIndex >= aliveCount)
			{
				throw new InvalidOperationException($"Index is not alive! SparseIndex: {sparseIndex}.");
			}

			TState state = _dense[denseIndex];

			int swapDenseIndex = aliveCount - 1;
			TState swapState = _dense[swapDenseIndex];
			int swapSparseIndex = swapState.SparseIndex;

			_sparse[sparseIndex] = swapDenseIndex;
			_sparse[swapSparseIndex] = denseIndex;

			swapState.SparseIndex = sparseIndex;
			state.SparseIndex = swapSparseIndex;

			_dense[denseIndex] = swapState;
			_dense[swapDenseIndex] = state;

			*_aliveCount -= 1;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public ref TState Get(int sparseIndex)
		{
			ThrowIfFrameIsNotCurrent();

			if (!IsAlive(sparseIndex))
			{
				throw new InvalidOperationException($"State does not exist! RequestedState: {sparseIndex}.");
			}

			int denseIndex = _sparse[sparseIndex];

			return ref _dense[denseIndex];
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Span<TState> GetAll()
		{
			ThrowIfFrameIsNotCurrent();

			return _dense.Slice(0, *_aliveCount);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool IsAlive(int sparseIndex)
		{
			ThrowIfFrameIsNotCurrent();

			return sparseIndex < _statesCapacity && _sparse[sparseIndex] < *_aliveCount;
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