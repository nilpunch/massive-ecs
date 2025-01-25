using System;
using System.Runtime.CompilerServices;
using Unity.IL2CPP.CompilerServices;

namespace Massive
{
	[Il2CppSetOption(Option.NullChecks, false)]
	[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
	[Il2CppSetOption(Option.DivideByZeroChecks, false)]
	public class SparseSet : PackedSet
	{
		private const int EndHole = int.MaxValue;

		/// <summary>
		/// The sparse array, mapping IDs to their packed indices.
		/// </summary>
		public int[] Sparse { get; private set; } = Array.Empty<int>();

		/// <summary>
		/// The current capacity of the packed array.
		/// </summary>
		public int PackedCapacity { get; private set; }

		/// <summary>
		/// The current capacity of the sparse array.
		/// </summary>
		public int SparseCapacity { get; private set; }

		/// <summary>
		/// The index of the next available hole in the packed array, or <see cref="EndHole"/> if no holes exist.
		/// </summary>
		private int NextHole { get; set; } = EndHole;

		public SparseSet(Packing packing = Packing.Continuous)
		{
			Packing = packing;
		}

		/// <summary>
		/// Checks whether a packed array has no holes in it.
		/// </summary>
		public bool IsContinuous
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => Packing == Packing.Continuous || NextHole == EndHole;
		}

		/// <summary>
		/// Checks whether a packed array has any holes in it.
		/// </summary>
		public bool HasHoles
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => Packing != Packing.Continuous && NextHole != EndHole;
		}

		/// <summary>
		/// Shoots only after <see cref="Assign"/> call, when the ID was not already assigned.
		/// </summary>
		public event Action<int> AfterAssigned;

		/// <summary>
		/// Shoots before each <see cref="Unassign"/> call, when the ID was asigned.
		/// </summary>
		public event Action<int> BeforeUnassigned;

		/// <summary>
		/// Gets or sets the current state for serialization or rollback purposes.
		/// </summary>
		public State CurrentState
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => new State(Count, NextHole, Packing);
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			set
			{
				Count = value.Count;
				NextHole = value.NextHole;
				Packing = value.Packing;
			}
		}

		/// <summary>
		/// Assigns an ID. If the ID is negative or already assigned, no action is performed.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Assign(int id)
		{
			// If ID is negative or already assigned, nothing to be done.
			if (id < 0 || id < SparseCapacity && Sparse[id] != Constants.InvalidId)
			{
				return;
			}

			EnsureSparseAt(id);

			if (Packing == Packing.WithHoles && NextHole != EndHole)
			{
				var index = NextHole;
				NextHole = ~Packed[index];
				Pair(id, index);
			}
			else // Packing.Continuous || Packing.WithPersistentHoles
			{
				EnsurePackedAt(Count);
				EnsureDataAt(Count);
				Pair(id, Count);
				Count += 1;
			}

			AfterAssigned?.Invoke(id);
		}

		/// <summary>
		/// Unassigns an ID. If the ID is negative or already unassigned, no action is performed.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Unassign(int id)
		{
			// If ID is negative or not assigned, nothing to be done.
			if (id < 0 || id >= SparseCapacity || Sparse[id] == Constants.InvalidId)
			{
				return;
			}

			BeforeUnassigned?.Invoke(id);

			if (Packing == Packing.Continuous)
			{
				Count -= 1;
				MoveAt(Count, Sparse[id]);
			}
			else // Packing.WithHoles || Packing.WithPersistentHoles
			{
				var index = Sparse[id];
				Packed[index] = ~NextHole;
				NextHole = index;
			}

			Sparse[id] = Constants.InvalidId;
		}

		/// <summary>
		/// Unassigns all IDs and triggers the <see cref="BeforeUnassigned"/> event for each one.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Clear()
		{
			if (IsContinuous)
			{
				for (var i = Count - 1; i >= 0; i--)
				{
					var id = Packed[i];
					BeforeUnassigned?.Invoke(id);
					Sparse[id] = Constants.InvalidId;
				}
			}
			else
			{
				for (var i = Count - 1; i >= 0; i--)
				{
					var id = Packed[i];
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

		/// <summary>
		/// Unassigns all IDs without triggering the <see cref="BeforeUnassigned"/> event.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void ClearWithoutNotify()
		{
			Array.Fill(Sparse, Constants.InvalidId);
			Count = 0;
			NextHole = EndHole;
		}

		/// <summary>
		/// Returns the packed index of the specified ID, or a negative value if the ID is not assigned.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public int GetIndexOrNegative(int id)
		{
			if (id < 0 || id >= SparseCapacity)
			{
				return Constants.InvalidId;
			}

			return Sparse[id];
		}

		/// <summary>
		/// Checks whether the specified ID is assigned.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool IsAssigned(int id)
		{
			return id >= 0 && id < SparseCapacity && Sparse[id] != Constants.InvalidId;
		}

		/// <summary>
		/// Checks whether the packed index is assigned.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool IsAssignedAt(int index)
		{
			return index >= 0 && index < PackedCapacity && Packed[index] >= 0;
		}

		/// <summary>
		/// Ensures the packed array has sufficient capacity for the specified index.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void EnsurePackedAt(int index)
		{
			if (index >= PackedCapacity)
			{
				ResizePacked(MathUtils.NextPowerOf2(index + 1));
			}
		}

		/// <summary>
		/// Ensures the sparse array has sufficient capacity for the specified index.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void EnsureSparseAt(int index)
		{
			if (index >= SparseCapacity)
			{
				ResizeSparse(MathUtils.NextPowerOf2(index + 1));
			}
		}

		/// <summary>
		/// Resizes the packed array to the specified capacity.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void ResizePacked(int capacity)
		{
			Packed = Packed.Resize(capacity);
			PackedCapacity = capacity;
		}

		/// <summary>
		/// Resizes the sparse array to the specified capacity.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void ResizeSparse(int capacity)
		{
			Sparse = Sparse.Resize(capacity);
			if (capacity > SparseCapacity)
			{
				Array.Fill(Sparse, Constants.InvalidId, SparseCapacity, capacity - SparseCapacity);
			}
			SparseCapacity = capacity;
		}

		/// <summary>
		/// Removes all holes from the packed array.
		/// </summary>
		public override void Compact()
		{
			if (HasHoles)
			{
				var count = Count;
				var nextHole = NextHole;

				for (; count > 0 && Packed[count - 1] < 0; count--) { }

				while (nextHole != EndHole)
				{
					var holeIndex = nextHole;
					nextHole = ~Packed[nextHole];
					if (holeIndex < count)
					{
						count -= 1;
						MoveAt(count, holeIndex);

						for (; count > 0 && Packed[count - 1] < 0; count--) { }
					}
				}

				Count = count;
				NextHole = EndHole;
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public PackedEnumerator GetEnumerator()
		{
			return new PackedEnumerator(this);
		}

		/// <summary>
		/// Swaps the positions of two elements in the packed array.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void SwapAt(int first, int second)
		{
			Debug.Assert(IsAssignedAt(first), ErrorMessage.InvalidIndex(first));
			Debug.Assert(IsAssignedAt(second), ErrorMessage.InvalidIndex(second));

			var firstId = Packed[first];
			var secondId = Packed[second];

			Pair(firstId, second);
			Pair(secondId, first);

			SwapDataAt(first, second);
		}

		/// <summary>
		/// Moves the data from one index to another.
		/// </summary>
		protected virtual void MoveDataAt(int source, int destination)
		{
		}

		/// <summary>
		/// Swaps the data between two indices.
		/// </summary>
		public virtual void SwapDataAt(int first, int second)
		{
		}

		/// <summary>
		/// Copies the data from one index to another.
		/// </summary>
		public virtual void CopyDataAt(int source, int destination)
		{
		}

		/// <summary>
		/// Ensures data exists at the specified index.
		/// </summary>
		public virtual void EnsureDataAt(int index)
		{
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void MoveAt(int source, int destination)
		{
			var sourceId = Packed[source];
			Pair(sourceId, destination);
			MoveDataAt(source, destination);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void Pair(int id, int index)
		{
			Sparse[id] = index;
			Packed[index] = id;
		}

		public readonly struct State
		{
			public readonly int Count;
			public readonly int NextHole;
			public readonly Packing Packing;

			public State(int count, int nextHole, Packing packing)
			{
				Count = count;
				NextHole = nextHole;
				Packing = packing;
			}
		}
	}
}
