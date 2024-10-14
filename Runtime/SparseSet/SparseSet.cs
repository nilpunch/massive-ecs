using System;
using System.Runtime.CompilerServices;
using Unity.IL2CPP.CompilerServices;

namespace Massive
{
	public enum IndexingMode
	{
		Packed,
		Direct,
	}

	[Il2CppSetOption(Option.NullChecks | Option.ArrayBoundsChecks, false)]
	public class SparseSet : ISet
	{
		private int[] _packed;
		private int[] _sparse;

		public IndexingMode IndexingMode { get; }
		public int Count { get; set; }

		public SparseSet(int setCapacity = Constants.DefaultCapacity, IndexingMode indexingMode = IndexingMode.Packed)
		{
			IndexingMode = indexingMode;
			_packed = indexingMode == IndexingMode.Packed ? new int[setCapacity] : Array.Empty<int>();
			_sparse = new int[setCapacity];

			Array.Fill(_sparse, Constants.InvalidId);
		}

		public bool IsPacked
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => IndexingMode == IndexingMode.Packed;
		}

		public int[] Packed => _packed;

		public int[] Sparse => _sparse;

		public int[] Ids
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => IsPacked ? Packed : Sparse;
		}

		public int PackedCapacity => Packed.Length;

		public int SparseCapacity => Sparse.Length;

		public event Action<int> AfterAssigned;

		public event Action<int> BeforeUnassigned;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Assign(int id)
		{
			// If ID is negative or element is alive, nothing to be done
			if (id < 0 || id < SparseCapacity && Sparse[id] != Constants.InvalidId)
			{
				return;
			}

			EnsureSparseForIndex(id);

			if (IsPacked)
			{
				EnsurePackedForIndex(Count);
				EnsureDataForIndex(Count);
				AssignIndex(id, Count);
				Count += 1;
			}
			else
			{
				EnsureDataForIndex(id);
				Sparse[id] = id;
				if (id >= Count)
				{
					Count = id + 1;
				}
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

			if (IsPacked)
			{
				Count -= 1;
				CopyFromToPacked(Count, Sparse[id]);
				Sparse[id] = Constants.InvalidId;
			}
			else
			{
				Sparse[id] = Constants.InvalidId;
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Clear()
		{
			if (IsPacked)
			{
				for (int i = Count - 1; i >= 0; i--)
				{
					BeforeUnassigned?.Invoke(Packed[i]);
					Sparse[Packed[i]] = Constants.InvalidId;
				}
			}
			else
			{
				for (int i = Count - 1; i >= 0; i--)
				{
					if (Sparse[i] != Constants.InvalidId)
					{
						BeforeUnassigned?.Invoke(Sparse[i]);
						Sparse[i] = Constants.InvalidId;
					}
				}
			}
			Count = 0;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public int GetIndex(int id)
		{
			return Sparse[id];
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public int GetIndexOrInvalid(int id)
		{
			if (id < 0 || id >= SparseCapacity)
			{
				return Constants.InvalidId;
			}

			return Sparse[id];
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool TryGetIndex(int id, out int index)
		{
			if (id < 0 || id >= SparseCapacity)
			{
				index = Constants.InvalidId;
				return false;
			}

			index = Sparse[id];

			return index != Constants.InvalidId;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool IsAssigned(int id)
		{
			return id >= 0 && id < SparseCapacity && Sparse[id] != Constants.InvalidId;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public virtual void SwapPacked(int packedA, int packedB)
		{
			int idA = Packed[packedA];
			int idB = Packed[packedB];
			AssignIndex(idA, packedB);
			AssignIndex(idB, packedA);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public virtual void ResizePacked(int capacity)
		{
			Array.Resize(ref _packed, capacity);
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
		protected virtual void CopyFromToPacked(int source, int destination)
		{
			int sourceId = Packed[source];
			AssignIndex(sourceId, destination);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		protected virtual void EnsureDataForIndex(int index)
		{
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void AssignIndex(int id, int packed)
		{
			Sparse[id] = packed;
			Packed[packed] = id;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void EnsurePackedForIndex(int index)
		{
			if (index >= PackedCapacity)
			{
				ResizePacked(MathHelpers.NextPowerOf2(index + 1));
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void EnsureSparseForIndex(int index)
		{
			if (index >= SparseCapacity)
			{
				ResizeSparse(MathHelpers.NextPowerOf2(index + 1));
			}
		}
	}
}
