using System;
using System.Runtime.CompilerServices;
using Unity.IL2CPP.CompilerServices;

namespace Massive
{
	[Il2CppSetOption(Option.NullChecks, false)]
	[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
	public readonly ref struct WorkableList<T>
	{
		private readonly WorkableChunk<T> _items;
		private readonly WorkableVar<int> _count;

		public WorkableList(ListHandle<T> list, ListAllocator<T> allocator)
		{
			_items = list.Items.In(allocator.Items);
			_count = list.Count.WorkWith(allocator.Count);
		}

		public ListHandle<T> Handle
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => new ListHandle<T>(_items.Handle, _count.Handle);
		}

		public ref T this[int index]
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				if (index >= Count)
				{
					throw new IndexOutOfRangeException();
				}

				return ref _items[index];
			}
		}

		public int Count
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => _count.Value;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Add(in T item)
		{
			ref var count = ref _count.Value;
			EnsureCapacityAt(count);
			_items[count++] = item;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void InsertAt(int index, in T item)
		{
			ref var count = ref _count.Value;
			if (index > count)
			{
				throw new IndexOutOfRangeException();
			}

			EnsureCapacityAt(count);

			_items.CopyToSelf(index, index + 1, count - index);
			_items[index] = item;
			count++;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void RemoveAt(int index)
		{
			ref var count = ref _count.Value;
			if (index >= count)
			{
				throw new IndexOutOfRangeException();
			}

			count--;
			_items.CopyToSelf(index + 1, index, count - index);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void RemoveAtSwap(int index)
		{
			ref var count = ref _count.Value;
			if (index >= count)
			{
				throw new IndexOutOfRangeException();
			}

			count--;
			var lastIndex = count;
			_items[index] = _items[lastIndex];
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Clear()
		{
			_count.Value = 0;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void EnsureCapacityAt(int index)
		{
			if (index >= _items.Length)
			{
				_items.Resize(MathUtils.NextPowerOf2(index + 1));
			}
		}

		public WorkableChunk<T>.Enumerator GetEnumerator() => new WorkableChunk<T>.Enumerator(_items, 0, Count);
	}
}
