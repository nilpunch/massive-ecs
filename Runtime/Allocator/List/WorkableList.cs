#if !MASSIVE_DISABLE_ASSERT
#define MASSIVE_ASSERT
#endif

using System.Runtime.CompilerServices;
using Unity.IL2CPP.CompilerServices;

namespace Massive
{
	[Il2CppSetOption(Option.NullChecks, false)]
	[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
	public readonly ref struct WorkableList<T> where T : unmanaged
	{
		private readonly WorkableArray<T> _items;
		private readonly WorkableVar<int> _count;

		public WorkableList(ChunkId items, ChunkId count, Allocator<T> itemsAllocator, Allocator<int> countAllocator)
		{
			_items = new WorkableArray<T>(items, itemsAllocator);
			_count = new WorkableVar<int>(count, countAllocator);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static implicit operator AllocatorListId(WorkableList<T> list)
		{
			return new AllocatorListId(list._items.ChunkId, list._count.ChunkId, list._items.Allocator.AllocatorId);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static implicit operator ListHandle<T>(WorkableList<T> list)
		{
			return new ListHandle<T>(list._items.ChunkId, list._count.ChunkId);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Free()
		{
			_items.Free();
			_count.Free();
		}

		public ref T this[int index]
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				AllocatorIndexOutOfRangeException.ThrowIfOutOfRangeExclusive(index, _count.Value);

				return ref _items.GetAtUnchecked(index);
			}
		}

		public int Count
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => _count.Value;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Add(T item)
		{
			ref var count = ref _count.Value;
			EnsureCapacityAt(count);
			_items[count++] = item;
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
		public void Insert(int index, T item)
		{
			ref var count = ref _count.Value;
			AllocatorIndexOutOfRangeException.ThrowIfOutOfRangeInclusive(index, count);

			EnsureCapacityAt(count);

			_items.CopyToSelf(index, index + 1, count - index);
			_items[index] = item;
			count++;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void RemoveAt(int index)
		{
			ref var count = ref _count.Value;
			AllocatorIndexOutOfRangeException.ThrowIfOutOfRangeExclusive(index, count);

			count--;
			_items.CopyToSelf(index + 1, index, count - index);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void RemoveAtSwapBack(int index)
		{
			ref var count = ref _count.Value;
			AllocatorIndexOutOfRangeException.ThrowIfOutOfRangeExclusive(index, count);

			count--;
			var lastIndex = count;
			_items[index] = _items[lastIndex];
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public int IndexOf(T item)
		{
			return _items.IndexOf(item, 0, Count);
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
				_items.Resize(index + 1, MemoryInit.Uninitialized);
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public WorkableArray<T>.Enumerator GetEnumerator()
		{
			return new WorkableArray<T>.Enumerator(_items, 0, Count);
		}
	}
}
