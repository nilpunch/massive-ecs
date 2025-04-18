﻿#if !MASSIVE_DISABLE_ASSERT
#define MASSIVE_ASSERT
#endif

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
		/// Don't cache it and use as is, underlying array can be resized at any moment.
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
		/// The maximum count of ids in use.
		/// </summary>
		public int UsedIds { get; private set; }

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
		/// Shoots only after <see cref="Add"/> call, when the ID was not already present.
		/// </summary>
		public event Action<int> AfterAdded;

		/// <summary>
		/// Shoots before each <see cref="Remove"/> call, when the ID was removed.
		/// </summary>
		public event Action<int> BeforeRemoved;

		/// <summary>
		/// Gets or sets the current state for serialization or rollback purposes.
		/// </summary>
		public State CurrentState
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => new State(Count, UsedIds, NextHole, Packing);
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			set
			{
				Count = value.Count;
				UsedIds = value.UsedIds;
				NextHole = value.NextHole;
				Packing = value.Packing;
			}
		}

		/// <summary>
		/// Adds an ID. If the ID is already added, no action is performed.
		/// </summary>
		/// <returns>
		/// True if the ID was added; false if it was already present.
		/// </returns>
		/// <remarks>
		/// Throws if provided ID is negative.
		/// </remarks>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool Add(int id)
		{
			Assert.NonNegative(id, nameof(id));

			if (id >= SparseCapacity)
			{
				ResizeSparse(MathUtils.NextPowerOf2(id + 1));
			}

			// If ID is already present, nothing to be done.
			if (Sparse[id] != Constants.InvalidId)
			{
				return false;
			}

			if (Packing == Packing.WithHoles && NextHole != EndHole)
			{
				// Fill the holes.
				var index = NextHole;
				NextHole = ~Packed[index];
				Pair(id, index);
			}
			else // if (Packing == Packing.Continuous || Packing == Packing.WithPersistentHoles)
			{
				// Append to the end.
				EnsurePackedAt(Count);
				EnsureDataAt(Count);
				Pair(id, Count);
				Count += 1;
			}

			UsedIds = MathUtils.Max(UsedIds, id + 1);

			AfterAdded?.Invoke(id);

			return true;
		}

		/// <summary>
		/// Removes an ID. If the ID is already removed, no action is performed.
		/// </summary>
		/// <returns>
		/// True if the ID was removed; false if it was not present.
		/// </returns>
		/// <remarks>
		/// Throws if provided ID is negative.
		/// </remarks>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool Remove(int id)
		{
			Assert.NonNegative(id, nameof(id));

			// If ID is not added, nothing to be done.
			if (id >= SparseCapacity || Sparse[id] == Constants.InvalidId)
			{
				return false;
			}

			BeforeRemoved?.Invoke(id);

			if (Packing == Packing.Continuous)
			{
				// Swap with last.
				Count -= 1;
				MoveAt(Count, Sparse[id]);
			}
			else // if (Packing == Packing.WithHoles || Packing == Packing.WithPersistentHoles)
			{
				// Create a hole.
				var index = Sparse[id];
				Packed[index] = ~NextHole;
				NextHole = index;
			}

			Sparse[id] = Constants.InvalidId;

			return true;
		}

		/// <summary>
		/// Removes all IDs and triggers the <see cref="BeforeRemoved"/> event for each one.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Clear()
		{
			if (IsContinuous)
			{
				for (var i = Count - 1; i >= 0; i--)
				{
					var id = Packed[i];
					BeforeRemoved?.Invoke(id);
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
						BeforeRemoved?.Invoke(id);
						Sparse[id] = Constants.InvalidId;
					}
				}
			}
			Count = 0;
			UsedIds = 0;
			NextHole = EndHole;
		}

		/// <summary>
		/// Removes all IDs without triggering the <see cref="BeforeRemoved"/> event.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void ClearWithoutNotify()
		{
			if (UsedIds > 0)
			{
				Array.Fill(Sparse, Constants.InvalidId, 0, UsedIds);
			}
			Count = 0;
			UsedIds = 0;
			NextHole = EndHole;
		}

		/// <summary>
		/// Returns the packed index of the specified ID,
		/// or a negative value if the ID is not added or is negative.
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
		/// Checks whether the specified ID is present.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool Has(int id)
		{
			return id >= 0 && id < SparseCapacity && Sparse[id] != Constants.InvalidId;
		}

		/// <summary>
		/// Checks whether the packed index is added.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool HasPacked(int index)
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
			Assert.HasPacked(this, first);
			Assert.HasPacked(this, second);

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

		/// <summary>
		/// Creates and returns a new sparse set that is an exact copy of this one.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public SparseSet CloneSparse()
		{
			var clone = new SparseSet();
			CopySparseTo(clone);
			return clone;
		}

		/// <summary>
		/// Copies all sparse state from this set into the specified one.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void CopySparseTo(SparseSet other)
		{
			other.EnsurePackedAt(Count - 1);
			other.EnsureSparseAt(UsedIds - 1);

			Array.Copy(Packed, other.Packed, Count);
			Array.Copy(Sparse, other.Sparse, UsedIds);

			if (UsedIds < other.UsedIds)
			{
				Array.Fill(other.Sparse, Constants.InvalidId, UsedIds, other.UsedIds - UsedIds);
			}

			other.CurrentState = CurrentState;
		}

		public readonly struct State
		{
			public readonly int Count;
			public readonly int UsedIds;
			public readonly int NextHole;
			public readonly Packing Packing;

			public State(int count, int usedIds, int nextHole, Packing packing)
			{
				Count = count;
				UsedIds = usedIds;
				NextHole = nextHole;
				Packing = packing;
			}
		}
	}
}
