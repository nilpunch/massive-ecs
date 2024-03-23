using System;
using System.Runtime.CompilerServices;
using Unity.IL2CPP.CompilerServices;

namespace Massive
{
	[Il2CppSetOption(Option.NullChecks, false)]
	[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
	public class SparseSet : ISet
	{
		public int[] Dense { get; }
		public int[] Sparse { get; }
		public int AliveCount { get; set; }

		public SparseSet(int dataCapacity = Constants.DataCapacity)
		{
			Dense = new int[dataCapacity];
			Sparse = new int[dataCapacity];
		}

		public ReadOnlySpan<int> AliveIds => new ReadOnlySpan<int>(Dense, 0, AliveCount);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public int Ensure(int id)
		{
			if (id < 0 || id >= Sparse.Length)
			{
				throw new ArgumentOutOfRangeException(nameof(id), id, $"Id must be in range [0, {Sparse.Length}).");
			}

			// If element is alive, nothing to be done
			if (TryGetDense(id, out var dense))
			{
				return dense;
			}

			int count = AliveCount;
			AliveCount += 1;

			AssignIndex(id, count);

			return count;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Delete(int id)
		{
			// If element is not alive, nothing to be done
			if (!TryGetDense(id, out var dense))
			{
				return;
			}

			int count = AliveCount;
			AliveCount -= 1;

			// If dense is the last used element, decreasing alive count is enough
			if (dense == count - 1)
			{
				return;
			}

			int lastElement = count - 1;
			CopyDense(lastElement, dense);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void DeleteDense(int dense)
		{
			int count = AliveCount;

			// If element is not alive, nothing to be done
			if (dense < 0 || dense >= count)
			{
				return;
			}

			AliveCount -= 1;

			// If dense is the last used element, decreasing alive count is enough
			if (dense == count - 1)
			{
				return;
			}

			int lastElement = count - 1;
			CopyDense(lastElement, dense);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool TryGetDense(int id, out int dense)
		{
			if (id < 0 || id >= Sparse.Length)
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
			if (id < 0 || id >= Sparse.Length)
			{
				return false;
			}

			int dense = Sparse[id];

			return dense < AliveCount && Dense[dense] == id;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public virtual void SwapDense(int denseA, int denseB)
		{
			int idA = Dense[denseA];
			int idB = Dense[denseB];
			AssignIndex(idA, denseB);
			AssignIndex(idB, denseA);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		protected virtual void CopyDense(int source, int destination)
		{
			int sourceId = Dense[source];
			Dense[destination] = sourceId;
			Sparse[sourceId] = destination;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void AssignIndex(int id, int dense)
		{
			Sparse[id] = dense;
			Dense[dense] = id;
		}
	}
}