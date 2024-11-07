using System;
using System.Runtime.CompilerServices;
using Unity.IL2CPP.CompilerServices;

namespace Massive
{
	public enum PackingMode
	{
		/// <summary>
		/// When an element is removed, its position is filled with the last element in the packed array.
		/// </summary>
		Continuous,

		/// <summary>
		/// When an element is removed, its position is left as a hole in the packed array.
		/// The holes are filled when new elements are added.
		/// </summary>
		WithHoles,
	}

	[Il2CppSetOption(Option.NullChecks, false)]
	[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
	[Il2CppSetOption(Option.DivideByZeroChecks, false)]
	public class SparseSet : IdsSource
	{
		private const int EndHole = int.MaxValue;

		private int[] _packed;
		private int[] _sparse;

		public int NextHole { get; set; }

		public SparseSet(PackingMode packingMode = PackingMode.Continuous)
		{
			_packed = Array.Empty<int>();
			_sparse = Array.Empty<int>();
			PackingMode = packingMode;

			NextHole = EndHole;
			Ids = _packed;
		}

		/// <summary>
		/// Checks whether a packed array has no holes in it.
		/// </summary>
		public bool IsContinuous
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => PackingMode == PackingMode.Continuous || NextHole == EndHole;
		}

		/// <summary>
		/// Checks whether a packed array has any holes in it.
		/// </summary>
		public bool HasHoles
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => PackingMode == PackingMode.WithHoles && NextHole != EndHole;
		}

		public int[] Packed => _packed;

		public int[] Sparse => _sparse;

		/// <summary>
		/// Shoots only after <see cref="Assign"/> call, when the id was not alive and therefore was created.
		/// </summary>
		public event Action<int> AfterAssigned;

		/// <summary>
		/// Shoots before each <see cref="Unassign"/> call, when the id was alive.
		/// </summary>
		public event Action<int> BeforeUnassigned;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Assign(int id)
		{
			// If ID is negative or element is alive, nothing to be done
			if (id < 0 || id < Sparse.Length && Sparse[id] != Constants.InvalidId)
			{
				return;
			}

			EnsureSparseForIndex(id);

			if (HasHoles)
			{
				int index = NextHole;
				NextHole = ~Packed[index];
				AssignIndex(id, index);
			}
			else
			{
				EnsurePackedForIndex(Count);
				EnsureDataForIndex(Count);
				AssignIndex(id, Count);
				Count += 1;
			}

			AfterAssigned?.Invoke(id);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Unassign(int id)
		{
			// If ID is negative or element is not alive, nothing to be done
			if (OutOfBounds(id) || Sparse[id] == Constants.InvalidId)
			{
				return;
			}

			BeforeUnassigned?.Invoke(id);

			if (PackingMode == PackingMode.Continuous)
			{
				Count -= 1;
				CopyFromToPacked(Count, Sparse[id]);
			}
			else
			{
				int index = Sparse[id];
				Packed[index] = ~NextHole;
				NextHole = index;
			}

			Sparse[id] = Constants.InvalidId;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Clear()
		{
			if (IsContinuous)
			{
				for (int i = Count - 1; i >= 0; i--)
				{
					int id = Packed[i];
					BeforeUnassigned?.Invoke(id);
					Sparse[id] = Constants.InvalidId;
				}
			}
			else
			{
				for (int i = Count - 1; i >= 0; i--)
				{
					int id = Packed[i];
					if (id >= 0)
					{
						BeforeUnassigned?.Invoke(id);
						Sparse[id] = Constants.InvalidId;
					}
				}
			}
			Count = 0;
			NextHole = EndHole;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void ClearWithoutNotify()
		{
			Array.Fill(Sparse, Constants.InvalidId);
			Count = 0;
			NextHole = EndHole;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public int GetIndex(int id)
		{
			return Sparse[id];
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public int GetIndexOrInvalid(int id)
		{
			if (OutOfBounds(id))
			{
				return Constants.InvalidId;
			}

			return Sparse[id];
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool TryGetIndex(int id, out int index)
		{
			if (OutOfBounds(id))
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
			return InsideBounds(id) && Sparse[id] != Constants.InvalidId;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public virtual void SwapPacked(int first, int second)
		{
			if (PackingMode == PackingMode.WithHoles)
			{
				throw new Exception("Swapping is not supported for packing mode with holes.");
			}

			int firstId = Packed[first];
			int secondId = Packed[second];
			AssignIndex(firstId, second);
			AssignIndex(secondId, first);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void EnsurePackedForIndex(int index)
		{
			if (index >= Packed.Length)
			{
				ResizePacked(MathHelpers.NextPowerOf2(index + 1));
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void EnsureSparseForIndex(int index)
		{
			if (index >= Sparse.Length)
			{
				ResizeSparse(MathHelpers.NextPowerOf2(index + 1));
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void ResizePacked(int capacity)
		{
			Array.Resize(ref _packed, capacity);
			Ids = _packed;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void ResizeSparse(int capacity)
		{
			int previousCapacity = Sparse.Length;
			Array.Resize(ref _sparse, capacity);
			if (capacity > previousCapacity)
			{
				Array.Fill(Sparse, Constants.InvalidId, previousCapacity, capacity - previousCapacity);
			}
		}

		public override void ChangePackingMode(PackingMode value)
		{
			if (value != PackingMode)
			{
				PackingMode = value;
				Compact();
			}
		}

		/// <summary>
		/// Removes all holes from the packed array.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Compact()
		{
			if (HasHoles)
			{
				for (; Count > 0 && Packed[Count - 1] < 0; Count--) { }

				while (NextHole != EndHole)
				{
					int holeIndex = NextHole;
					NextHole = ~Packed[NextHole];
					if (holeIndex < Count)
					{
						Count -= 1;
						CopyFromToPacked(Count, holeIndex);

						for (; Count > 0 && Packed[Count - 1] < 0; Count--) { }
					}
				}
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public IdsSourceEnumerator GetEnumerator()
		{
			return new IdsSourceEnumerator(this);
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
		private void AssignIndex(int id, int index)
		{
			Sparse[id] = index;
			Packed[index] = id;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private bool InsideBounds(int id)
		{
			return id >= 0 && id < Sparse.Length;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private bool OutOfBounds(int id)
		{
			return id < 0 || id >= Sparse.Length;
		}
	}
}
