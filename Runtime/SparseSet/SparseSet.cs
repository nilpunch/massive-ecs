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

		public int Count { get; set; }

		public SparseSet(int dataCapacity = Constants.DataCapacity)
		{
			_dense = new int[dataCapacity];
			_sparse = new int[dataCapacity];
		}

		public ReadOnlySpan<int> Ids => new ReadOnlySpan<int>(Dense, 0, Count);

		public int DenseCapacity => Dense.Length;

		public int SparseCapacity => Sparse.Length;

		public event Action<int> AfterAssigned;

		public event Action<int> BeforeUnassigned;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Assign(int id)
		{
			if (id < 0)
			{
				throw new ArgumentOutOfRangeException(nameof(id), id, $"Id must be positive.");
			}

			// If element is alive, nothing to be done
			if (IsAssigned(id))
			{
				return;
			}

			int count = Count;
			Count += 1;

			if (id >= SparseCapacity)
			{
				GrowSparse(id + 1);
			}

			if (count >= DenseCapacity)
			{
				GrowDense(count + 1);
			}

			AssignIndex(id, count);

			AfterAssigned?.Invoke(id);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Unassign(int id)
		{
			BeforeUnassigned?.Invoke(id);

			// If element is not alive, nothing to be done
			if (!TryGetDense(id, out var dense))
			{
				return;
			}

			int count = Count;
			Count -= 1;

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
			var ids = Ids;
			for (int i = ids.Length - 1; i >= 0; i--)
			{
				BeforeUnassigned?.Invoke(ids[i]);
				Count -= 1;
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

			return dense < Count && Dense[dense] == id;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool IsAssigned(int id)
		{
			if (id < 0 || id >= SparseCapacity)
			{
				return false;
			}

			int dense = Sparse[id];

			return dense < Count && Dense[dense] == id;
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