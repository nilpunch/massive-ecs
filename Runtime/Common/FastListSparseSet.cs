using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.IL2CPP.CompilerServices;

namespace Massive
{
	[Il2CppSetOption(Option.NullChecks | Option.ArrayBoundsChecks, false)]
	public class FastListSparseSet
	{
		private SparseSet[] _items = Array.Empty<SparseSet>();

		public int Count { get; private set; }

		public int Capacity
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => Items.Length;
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			set => Array.Resize(ref _items, value);
		}

		public Span<SparseSet> Span
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => new Span<SparseSet>(Items, 0, Count);
		}

		public ReadOnlySpan<SparseSet> ReadOnlySpan
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => new ReadOnlySpan<SparseSet>(Items, 0, Count);
		}

		public SparseSet[] Items
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => _items;
		}

		public ref SparseSet this[int index]
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => ref Items[index];
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Add(SparseSet item)
		{
			EnsureCapacity(Count + 1);

			Items[Count++] = item;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool Remove(SparseSet item)
		{
			int index = IndexOf(item);
			if (index >= 0)
			{
				RemoveAt(index);
				return true;
			}

			return false;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void RemoveAt(int index)
		{
			Count -= 1;

			if (index < Count)
			{
				Array.Copy(Items, index + 1, Items, index, Count - index);
			}

			Items[Count] = default;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public int IndexOf(SparseSet item)
		{
			return Array.IndexOf(Items, item, 0, Count);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Insert(int index, SparseSet item)
		{
			EnsureCapacity(Count + 1);

			if (index < Count)
			{
				Array.Copy(Items, index, Items, index + 1, Count - index);
			}

			Items[index] = item;
			Count++;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Clear()
		{
			if (Count > 0)
			{
				Array.Clear(Items, 0, Count);
				Count = 0;
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public int BinarySearch(SparseSet item)
		{
			return BinarySearch(0, Count, item, null);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public int BinarySearch(SparseSet item, IComparer<SparseSet> comparer)
		{
			return BinarySearch(0, Count, item, comparer);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public int BinarySearch(int index, int count, SparseSet item, IComparer<SparseSet> comparer)
		{
			return Array.BinarySearch(Items, index, count, item, comparer ?? Comparer<SparseSet>.Default);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Enumerator GetEnumerator()
		{
			return new Enumerator(this);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void EnsureCapacity(int min)
		{
			if (Capacity < min)
			{
				Capacity = MathHelpers.NextPowerOf2(min);
			}
		}

		public struct Enumerator
		{
			private readonly SparseSet[] _items;
			private readonly int _count;
			private int _index;

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public Enumerator(FastListSparseSet fastList)
			{
				_items = fastList.Items;
				_count = fastList.Count;
				_index = -1;
			}

			public SparseSet Current
			{
				[MethodImpl(MethodImplOptions.AggressiveInlining)]
				get => _items[_index];
			}

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public bool MoveNext()
			{
				return ++_index < _count;
			}
		}
	}
}
