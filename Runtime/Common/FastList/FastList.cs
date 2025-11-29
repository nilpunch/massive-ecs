using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.IL2CPP.CompilerServices;

namespace Massive
{
	[Il2CppSetOption(Option.NullChecks, false)]
	[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
	public class FastList<T>
	{
		public T[] Items { get; private set; } = Array.Empty<T>();

		public int Count { get; protected set; }

		public int Capacity
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => Items.Length;
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			set => Items = Items.Resize(value);
		}

		public Span<T> Span
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => new Span<T>(Items, 0, Count);
		}

		public ReadOnlySpan<T> ReadOnlySpan
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => new ReadOnlySpan<T>(Items, 0, Count);
		}

		public ref T this[int index]
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => ref Items[index];
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Add(T item)
		{
			EnsureCapacity(Count + 1);

			Items[Count++] = item;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool Remove(T item)
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
				Array.Copy(Items, index + 1, Items, index, Count - index);
			}

			Items[Count] = default;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public int IndexOf(T item)
		{
			return Array.IndexOf(Items, item, 0, Count);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Insert(int index, T item)
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
			Count = 0;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public int BinarySearch(T item)
		{
			return BinarySearch(0, Count, item, null);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public int BinarySearch(T item, IComparer<T> comparer)
		{
			return BinarySearch(0, Count, item, comparer);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public int BinarySearch(int index, int count, T item, IComparer<T> comparer)
		{
			return Array.BinarySearch(Items, index, count, item, comparer ?? Comparer<T>.Default);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Enumerator GetEnumerator()
		{
			return new Enumerator(Items, Count);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void EnsureCapacity(int min)
		{
			if (Capacity < min)
			{
				Capacity = MathUtils.RoundUpToPowerOfTwo(min);
			}
		}

		[Il2CppSetOption(Option.NullChecks, false)]
		[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
		public struct Enumerator
		{
			private T[] _data;
			private int _index;

			public Enumerator(T[] data, int count)
			{
				_data = data;
				_index = count;
			}

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public bool MoveNext()
			{
				return --_index >= 0;
			}

			public ref T Current
			{
				[MethodImpl(MethodImplOptions.AggressiveInlining)]
				get => ref _data[_index];
			}
		}
	}
}
