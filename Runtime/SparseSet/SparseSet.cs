using System;
using System.Runtime.CompilerServices;
using Unity.IL2CPP.CompilerServices;

namespace Massive
{
	[Il2CppSetOption(Option.NullChecks | Option.ArrayBoundsChecks, false)]
	public class SparseSet : ISet
	{
		private int[] _dense;
		private int[] _sparse;

		public bool InPlace { get; }
		public int Count { get; set; }

		public SparseSet(int setCapacity = Constants.DefaultCapacity, bool inPlace = false)
		{
			InPlace = inPlace;
			_dense = inPlace ? Array.Empty<int>() : new int[setCapacity];
			_sparse = new int[setCapacity];

			Array.Fill(_sparse, Constants.InvalidId);
		}

		public int[] Dense
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => _dense;
		}

		public int[] Sparse
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => _sparse;
		}

		public ReadOnlySpan<int> Ids
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => InPlace ? new ReadOnlySpan<int>(Sparse, 0, Count) : new ReadOnlySpan<int>(Dense, 0, Count);
		}

		public int DenseCapacity
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => Dense.Length;
		}

		public int SparseCapacity
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => Sparse.Length;
		}

		public event Action<int> AfterAssigned;

		public event Action<int> BeforeUnassigned;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public virtual void Assign(int id)
		{
			// If ID is negative or element is alive, nothing to be done
			if (id < 0 || id < SparseCapacity && Sparse[id] != Constants.InvalidId)
			{
				return;
			}

			if (InPlace)
			{
				if (id >= Count)
				{
					Count = id + 1;
					EnsureSparseCapacity(id + 1);
				}

				Sparse[id] = id;
			}
			else
			{
				EnsureSparseCapacity(id + 1);
				EnsureDenseCapacity(Count + 1);

				AssignIndex(id, Count);
				Count += 1;
			}

			AfterAssigned?.Invoke(id);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Unassign(int id)
		{
			// If ID is negative or element is not alive, nothing to be done
			if (id < 0 || id >= SparseCapacity || Sparse[id] == Constants.InvalidId)
			{
				return;
			}

			BeforeUnassigned?.Invoke(id);

			if (InPlace)
			{
				Sparse[id] = Constants.InvalidId;
			}
			else
			{
				Count -= 1;
				CopyFromToDense(Count, Sparse[id]);
				Sparse[id] = Constants.InvalidId;
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Clear()
		{
			var ids = Ids;
			if (InPlace)
			{
				for (int i = ids.Length - 1; i >= 0; i--)
				{
					if (ids[i] != Constants.InvalidId)
					{
						BeforeUnassigned?.Invoke(ids[i]);
						Count = i;
						Sparse[ids[i]] = Constants.InvalidId;
					}
				}
				Count = 0;
			}
			else
			{
				for (int i = ids.Length - 1; i >= 0; i--)
				{
					BeforeUnassigned?.Invoke(ids[i]);
					Count -= 1;
					Sparse[ids[i]] = Constants.InvalidId;
				}
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public int GetDense(int id)
		{
			return Sparse[id];
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public int GetDenseOrInvalid(int id)
		{
			if (id < 0 || id >= SparseCapacity)
			{
				return Constants.InvalidId;
			}

			return Sparse[id];
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool TryGetDense(int id, out int dense)
		{
			if (id < 0 || id >= SparseCapacity)
			{
				dense = Constants.InvalidId;
				return false;
			}

			dense = Sparse[id];

			return dense != Constants.InvalidId;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool IsAssigned(int id)
		{
			return id >= 0 && id < SparseCapacity && Sparse[id] != Constants.InvalidId;
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
			int previousCapacity = SparseCapacity;
			Array.Resize(ref _sparse, capacity);

			if (capacity > previousCapacity)
			{
				Array.Fill(_sparse, Constants.InvalidId, previousCapacity, capacity - previousCapacity);
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		protected virtual void CopyFromToDense(int source, int destination)
		{
			int sourceId = Dense[source];
			AssignIndex(sourceId, destination);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void AssignIndex(int id, int dense)
		{
			Sparse[id] = dense;
			Dense[dense] = id;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void EnsureDenseCapacity(int capacity)
		{
			if (capacity > DenseCapacity)
			{
				ResizeDense(MathHelpers.NextPowerOf2(capacity));
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void EnsureSparseCapacity(int capacity)
		{
			if (capacity > SparseCapacity)
			{
				ResizeSparse(MathHelpers.NextPowerOf2(capacity));
			}
		}
	}
}
