using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.IL2CPP.CompilerServices;

namespace Massive
{
	[Il2CppSetOption(Option.NullChecks, false)]
	[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
	public class BitSetList
	{
		private BitSet[] _items = Array.Empty<BitSet>();

		public int Count { get; private set; }

		public int Capacity
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => _items.Length;
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			set => Array.Resize(ref _items, value);
		}

		public Span<BitSet> Span
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => new Span<BitSet>(_items, 0, Count);
		}

		public ReadOnlySpan<BitSet> ReadOnlySpan
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => new ReadOnlySpan<BitSet>(_items, 0, Count);
		}

		public BitSet[] Items
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => _items;
		}

		public ref BitSet this[int index]
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => ref _items[index];
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Add(BitSet item)
		{
			EnsureCapacity(Count + 1);

			_items[Count++] = item;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool Remove(BitSet item)
		{
			var index = IndexOf(item);
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
				Array.Copy(_items, index + 1, _items, index, Count - index);
			}

			_items[Count] = default;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public int IndexOf(BitSet item)
		{
			return Array.IndexOf(_items, item, 0, Count);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Insert(int index, BitSet item)
		{
			EnsureCapacity(Count + 1);

			if (index < Count)
			{
				Array.Copy(_items, index, _items, index + 1, Count - index);
			}

			_items[index] = item;
			Count++;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public int BinarySearch(BitSet item)
		{
			return BinarySearch(0, Count, item, null);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public int BinarySearch(BitSet item, IComparer<BitSet> comparer)
		{
			return BinarySearch(0, Count, item, comparer);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public int BinarySearch(int index, int count, BitSet item, IComparer<BitSet> comparer)
		{
			return Array.BinarySearch(_items, index, count, item, comparer ?? Comparer<BitSet>.Default);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Span<BitSet>.Enumerator GetEnumerator()
		{
			return new Span<BitSet>(_items, 0, Count).GetEnumerator();
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void EnsureCapacity(int min)
		{
			if (Capacity < min)
			{
				Capacity = MathUtils.RoundUpToPowerOfTwo(min);
			}
		}
	}
}
