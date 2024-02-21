using System;
using System.Runtime.CompilerServices;

namespace MassiveData
{
	[Unity.IL2CPP.CompilerServices.Il2CppSetOption(Unity.IL2CPP.CompilerServices.Option.NullChecks, false)]
	[Unity.IL2CPP.CompilerServices.Il2CppSetOption(Unity.IL2CPP.CompilerServices.Option.ArrayBoundsChecks, false)]
	[Unity.IL2CPP.CompilerServices.Il2CppSetOption(Unity.IL2CPP.CompilerServices.Option.DivideByZeroChecks, false)]
	public class SparseSet
	{
		protected int[] Dense { get; }
		protected int[] Sparse { get; }
		protected int MaxDense { get; set; }
		protected int MaxId { get; set; }
		public int AliveCount { get; protected set; }

		public SparseSet(int dataCapacity = 100)
		{
			Dense = new int[dataCapacity];
			Sparse = new int[dataCapacity];
		}

		public ReadOnlySpan<int> AliveIds => new ReadOnlySpan<int>(Dense, 0, AliveCount);

		public MassiveCreateInfo Ensure(int id)
		{
			if (id >= Sparse.Length)
			{
				throw new InvalidOperationException($"Exceeded limit of ids! Limit: {Sparse.Length}.");
			}

			int count = AliveCount;
			int dense = Sparse[id];

			// Check if element is paired
			if (Dense[dense] == id)
			{
				// If dense is already alive, nothing to be done
				if (dense < count)
				{
					return new MassiveCreateInfo() { Id = id, Dense = dense };
				}

				// If dense is not alive, swap it with the first unused element
				SwapDense(dense, count);

				// First unused element is now last used element
				AliveCount += 1;

				return new MassiveCreateInfo() { Id = id, Dense = count };
			}

			// Add new element to dense array and pair it with sparse
			MaxDense += 1;
			AliveCount += 1;
			MaxId = Math.Max(MaxId, id + 1);

			// If there are unused elements in the dense,
			// move the first unused element to the end
			int lastDense = MaxDense - 1;
			if (count < lastDense)
			{
				int unusedId = Dense[count];
				AssignIndex(unusedId, lastDense);
			}

			AssignIndex(id, count);

			return new MassiveCreateInfo() { Id = id, Dense = count };
		}

		public MassiveCreateInfo Create()
		{
			int count = AliveCount;
			if (count == Dense.Length)
			{
				throw new InvalidOperationException($"Exceeded limit of data! Limit: {Dense.Length}.");
			}

			// If there are unused elements in the dense array, return last
			var maxDense = MaxDense;
			if (count < maxDense)
			{
				AliveCount += 1;
				return new MassiveCreateInfo() { Id = Dense[count], Dense = count };
			}

			var maxId = MaxId;
			if (maxId == Sparse.Length)
			{
				throw new InvalidOperationException($"Exceeded limit of ids! Limit: {Sparse.Length}.");
			}

			// Add new element to dense array and pair it with a new element from sparse
			AliveCount += 1;
			MaxId += 1;
			MaxDense += 1;

			AssignIndex(maxId, maxDense);

			return new MassiveCreateInfo() { Id = maxId, Dense = maxDense };
		}

		public MassiveDeleteInfo? Delete(int id)
		{
			int aliveCount = AliveCount;

			// If element is not alive, nothing to be done
			if (!TryGetDense(id, out var dense))
			{
				return default;
			}

			AliveCount -= 1;

			// If dense is the last used element, nothing to be done
			if (dense == aliveCount - 1)
			{
				return default;
			}

			int swapDense = aliveCount - 1;
			SwapDense(dense, swapDense);

			return new MassiveDeleteInfo() { DenseSwapTarget = dense, DenseSwapSource = swapDense };
		}

		public MassiveDeleteInfo? DeleteDense(int dense)
		{
			int aliveCount = AliveCount;

			// Element is not alive, nothing to be done
			if (dense >= aliveCount)
			{
				return default;
			}

			AliveCount -= 1;

			// If dense is the last used element, nothing to be done
			if (dense == aliveCount - 1)
			{
				return default;
			}

			int swapDense = aliveCount - 1;
			SwapDense(dense, swapDense);

			return new MassiveDeleteInfo() { DenseSwapTarget = dense, DenseSwapSource = swapDense };
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public int GetDense(int id)
		{
			return Sparse[id];
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool TryGetDense(int id, out int dense)
		{
			if (id >= MaxId)
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
			if (id >= MaxId)
				return false;

			int dense = Sparse[id];

			return dense < AliveCount && Dense[dense] == id;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void SwapDense(int denseA, int denseB)
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