using System;
using System.Runtime.CompilerServices;
using Unity.IL2CPP.CompilerServices;

namespace Massive
{
	public enum Packing : byte
	{
		/// <summary>
		/// When an element is removed, its position is filled with the last element in the packed array.
		/// </summary>
		Continuous,

		/// <summary>
		/// When an element is removed, its position is left as a hole in the packed array.
		/// Holes are filled automatically when new elements are added.
		/// </summary>
		WithHoles,

		/// <summary>
		/// When an element is removed, its position is left as a hole in the packed array.
		/// Holes persist until manually compacted.
		/// </summary>
		WithPersistentHoles,
	}

	[Il2CppSetOption(Option.NullChecks, false)]
	[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
	[Il2CppSetOption(Option.DivideByZeroChecks, false)]
	public class SparseSet : IdsSource
	{
		private const int EndHole = int.MaxValue;

		private int[] _packed = Array.Empty<int>();
		private int[] _sparse = Array.Empty<int>();

		private int NextHole { get; set; } = EndHole;

		public SparseSet(Packing packing = Packing.Continuous)
		{
			Packing = packing;
			Ids = _packed;
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

		/// <summary>
		/// For serialization and rollbacks only.
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

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Assign(int id)
		{
			// If ID is negative or element is alive, nothing to be done
			if (id < 0 || id < Sparse.Length && Sparse[id] != Constants.InvalidId)
			{
				return;
			}

			EnsureSparseForIndex(id);

			switch (Packing)
			{
				case Packing.WithHoles:
					if (NextHole != EndHole)
					{
						var index = NextHole;
						NextHole = ~Packed[index];
						AssignIndex(id, index);
						break;
					}
					goto case Packing.Continuous;

				case Packing.Continuous:
				case Packing.WithPersistentHoles:
					EnsurePackedForIndex(Count);
					EnsureDataForIndex(Count);
					AssignIndex(id, Count);
					Count += 1;
					break;
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

			switch (Packing)
			{
				case Packing.Continuous:
					Count -= 1;
					CopyFromToPacked(Count, Sparse[id]);
					break;

				case Packing.WithHoles:
				case Packing.WithPersistentHoles:
					var index = Sparse[id];
					Packed[index] = ~NextHole;
					NextHole = index;
					break;
			}

			Sparse[id] = Constants.InvalidId;
		}

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
		/// For deserialization only.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void ClearWithoutNotify()
		{
			Array.Fill(Sparse, Constants.InvalidId);
			Count = 0;
			NextHole = EndHole;
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
		public bool IsAssigned(int id)
		{
			return InsideBounds(id) && Sparse[id] != Constants.InvalidId;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public virtual void SwapPacked(int first, int second)
		{
			if (HasHoles)
			{
				throw new Exception("Swapping is not supported with holes. Use Compact() before swapping.");
			}

			var firstId = Packed[first];
			var secondId = Packed[second];
			AssignIndex(firstId, second);
			AssignIndex(secondId, first);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void EnsurePackedForIndex(int index)
		{
			if (index >= Packed.Length)
			{
				ResizePacked(MathUtils.NextPowerOf2(index + 1));
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void EnsureSparseForIndex(int index)
		{
			if (index >= Sparse.Length)
			{
				ResizeSparse(MathUtils.NextPowerOf2(index + 1));
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
			var previousCapacity = Sparse.Length;
			Array.Resize(ref _sparse, capacity);
			if (capacity > previousCapacity)
			{
				Array.Fill(Sparse, Constants.InvalidId, previousCapacity, capacity - previousCapacity);
			}
		}

		public override Packing ExchangePacking(Packing packing)
		{
			var previousPacking = Packing;
			if (packing != Packing)
			{
				Packing = packing;
				if (packing == Packing.Continuous)
				{
					Compact();
				}
			}
			return previousPacking;
		}

		/// <summary>
		/// Removes all holes from the packed array.
		/// </summary>
		public void Compact()
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
						CopyFromToPacked(count, holeIndex);

						for (; count > 0 && Packed[count - 1] < 0; count--) { }
					}
				}

				Count = count;
				NextHole = EndHole;
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public IdsEnumerator GetEnumerator()
		{
			return new IdsEnumerator(this);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		protected virtual void CopyDataFromToPacked(int source, int destination)
		{
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		protected virtual void EnsureDataForIndex(int index)
		{
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void CopyFromToPacked(int source, int destination)
		{
			var sourceId = Packed[source];
			AssignIndex(sourceId, destination);
			CopyDataFromToPacked(source, destination);
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
