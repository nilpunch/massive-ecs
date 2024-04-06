using System;
using System.Runtime.CompilerServices;
using Unity.IL2CPP.CompilerServices;

namespace Massive
{
	[Il2CppSetOption(Option.NullChecks, false)]
	[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
	public class SparseSet : ISet
	{
		private int[] _dense;
		private int[] _sparse;

		public int[] Dense => _dense;

		public int[] Sparse => _sparse;

		public int AliveCount { get; set; }

		public SparseSet(int dataCapacity = Constants.DataCapacity)
		{
			_dense = new int[dataCapacity];
			_sparse = new int[dataCapacity];
		}

		public ReadOnlySpan<int> AliveIds => new ReadOnlySpan<int>(Dense, 0, AliveCount);

		public int DenseCapacity => Dense.Length;

		public int SparseCapacity => Sparse.Length;

		public event Action<int> AfterAdded;

		public event Action<int> BeforeRemoved;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Ensure(int id)
		{
			if (id < 0)
			{
				throw new ArgumentOutOfRangeException(nameof(id), id, $"Id must be positive.");
			}

			// If element is alive, nothing to be done
			if (IsAlive(id))
			{
				return;
			}

			int count = AliveCount;
			AliveCount += 1;

			if (id >= SparseCapacity)
			{
				GrowSparse(id + 1);
			}

			if (count >= DenseCapacity)
			{
				GrowDense(count + 1);
			}

			AssignIndex(id, count);

			AfterAdded?.Invoke(id);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Remove(int id)
		{
			BeforeRemoved?.Invoke(id);

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
			CopyFromToDense(lastElement, dense);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Clear()
		{
			var ids = AliveIds;
			for (int i = ids.Length - 1; i >= 0; i--)
			{
				BeforeRemoved?.Invoke(ids[i]);
				AliveCount -= 1;
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public int GetDense(int id)
		{
			return Sparse[id];
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool TryGetDense(int id, out int dense)
		{
			if (id < 0 || id >= SparseCapacity)
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
			if (id < 0 || id >= SparseCapacity)
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
		public virtual void ResizeDense(int capacity)
		{
			Array.Resize(ref _dense, capacity);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public virtual void ResizeSparse(int capacity)
		{
			Array.Resize(ref _sparse, capacity);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		protected virtual void CopyFromToDense(int source, int destination)
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

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void GrowDense(int desiredCapacity)
		{
			ResizeDense(MathHelpers.GetNextPowerOf2(desiredCapacity));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void GrowSparse(int desiredCapacity)
		{
			ResizeSparse(MathHelpers.GetNextPowerOf2(desiredCapacity));
		}
	}
}