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
		private int[] _sparse = new int[] { Constants.InvalidId };

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

			EnsureSparseAt(id);

			if (Packing == Packing.WithHoles && NextHole != EndHole)
			{
				var index = NextHole;
				NextHole = ~Packed[index];
				AssignIndex(id, index);
			}
			else
			{
				EnsurePackedAt(Count);
				EnsureDataAt(Count);
				AssignIndex(id, Count);
				Count += 1;
			}

			AfterAssigned?.Invoke(id);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Unassign(int id)
		{
			// If ID is negative or element is not alive, nothing to be done
			if (!IsAssigned(id))
			{
				return;
			}

			BeforeUnassigned?.Invoke(id);

			if (Packing == Packing.Continuous)
			{
				Count -= 1;
				CopyAt(Count, Sparse[id]);
			}
			else
			{
				var index = Sparse[id];
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
			int negativeIfIdOk = ~id;
			int negativeIfBoundsOk = id - Sparse.Length;
			int oneIfOkElseZero = (int)((uint)negativeIfBoundsOk >> 31) & (int)((uint)negativeIfIdOk >> 31);
			int negativeOneIfOkElseZero = -oneIfOkElseZero;
			return ~negativeOneIfOkElseZero | Sparse[id & negativeOneIfOkElseZero];
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool IsAssigned(int id)
		{
			return GetIndexOrInvalid(id) >= 0;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void EnsurePackedAt(int index)
		{
			if (index >= Packed.Length)
			{
				ResizePacked(MathUtils.NextPowerOf2(index + 1));
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void EnsureSparseAt(int index)
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
				if (packing == Packing.Continuous)
				{
					Compact();
				}
				Packing = packing;
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
						CopyAt(count, holeIndex);

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
		public void SwapAt(int first, int second)
		{
			var firstId = Packed[first];
			var secondId = Packed[second];

			ThrowIfNegative(firstId, $"Can't swap negative id.");
			ThrowIfNegative(secondId, $"Can't swap negative id.");

			AssignIndex(firstId, second);
			AssignIndex(secondId, first);

			SwapDataAt(first, second);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public virtual void SwapDataAt(int first, int second)
		{
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public virtual void CopyDataAt(int source, int destination)
		{
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public virtual void EnsureDataAt(int index)
		{
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void CopyAt(int source, int destination)
		{
			var sourceId = Packed[source];
			AssignIndex(sourceId, destination);
			SwapDataAt(source, destination);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void AssignIndex(int id, int index)
		{
			Sparse[id] = index;
			Packed[index] = id;
		}

		private void ThrowIfNegative(int id, string errorMessage)
		{
			if (id < 0)
			{
				throw new Exception(errorMessage);
			}
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
