using System;
using System.Runtime.CompilerServices;
using Unity.IL2CPP.CompilerServices;

namespace Massive
{
	[Il2CppSetOption(Option.NullChecks, false)]
	[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
	[Il2CppSetOption(Option.DivideByZeroChecks, false)]
	public class SparseSet : ISet
	{
		public int[] Dense { get; }
		public int[] Sparse { get; }
		public int AliveCount { get; set; }

		public SparseSet(int dataCapacity = Constants.DataCapacity)
		{
			Capacity = dataCapacity;
			Dense = new int[dataCapacity];
			Sparse = new int[dataCapacity];
		}

		public int Capacity { get; }

		public ReadOnlySpan<int> AliveIds => new ReadOnlySpan<int>(Dense, 0, AliveCount);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public CreateInfo Ensure(int id)
		{
			if (id >= Sparse.Length)
			{
				throw new InvalidOperationException($"Exceeded limit of ids! Limit: {Sparse.Length}.");
			}

			int count = AliveCount;
			int dense = Sparse[id];

			// Check if element is alive then do nothing
			if (dense < AliveCount && Dense[dense] == id)
			{
				return new CreateInfo() { Id = id, Dense = dense };
			}

			AliveCount += 1;

			AssignIndex(id, count);

			return new CreateInfo() { Id = id, Dense = count };
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public DeleteInfo? Delete(int id)
		{
			int aliveCount = AliveCount;

			// If element is not alive, nothing to be done
			if (!TryGetDense(id, out var dense))
			{
				return default;
			}

			AliveCount -= 1;

			// If dense is the last used element, decreasing alive count is enough
			if (dense == aliveCount - 1)
			{
				return default;
			}

			int swapDense = aliveCount - 1;
			SwapDense(dense, swapDense);

			return new DeleteInfo() { DenseSwapTarget = dense, DenseSwapSource = swapDense };
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public DeleteInfo? DeleteDense(int dense)
		{
			int aliveCount = AliveCount;

			// Element is not alive, nothing to be done
			if (dense >= aliveCount)
			{
				return default;
			}

			AliveCount -= 1;

			// If dense is the last used element, decreasing alive count is enough
			if (dense == aliveCount - 1)
			{
				return default;
			}

			int swapDense = aliveCount - 1;
			SwapDense(dense, swapDense);

			return new DeleteInfo() { DenseSwapTarget = dense, DenseSwapSource = swapDense };
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public int GetDense(int id)
		{
			return Sparse[id];
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool TryGetDense(int id, out int dense)
		{
			if (id >= Sparse.Length)
			{
				dense = default;
				return false;
			}

			dense = Sparse[id];

			return dense < AliveCount && Dense[dense] == id;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool IsAlive(int id)
		{
			if (id >= Sparse.Length)
				return false;

			int dense = Sparse[id];

			return dense < AliveCount && Dense[dense] == id;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void SwapDense(int denseA, int denseB)
		{
			int idA = Dense[denseA];
			int idB = Dense[denseB];
			AssignIndex(idA, denseB);
			AssignIndex(idB, denseA);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void AssignIndex(int id, int dense)
		{
			Sparse[id] = dense;
			Dense[dense] = id;
		}
	}
}