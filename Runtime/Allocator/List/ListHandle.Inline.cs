using System.Runtime.CompilerServices;

namespace Massive
{
	public readonly unsafe partial struct ListHandle<T>
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Free(Allocator allocator)
		{
			allocator.Free(ItemsId.ChunkId);
			allocator.Free(CountId.ChunkId);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public ListHandle<T> Track(Allocator allocator, int id)
		{
			allocator.Track(id, ItemsId.ChunkId);
			allocator.Track(id, CountId.ChunkId);
			return this;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public ref T GetAt(Allocator allocator, int index)
		{
			return ref ItemsId.GetAt(allocator, index);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public ref T GetAtUnchecked(Allocator allocator, int index)
		{
			return ref ItemsId.GetAtUnchecked(allocator, index);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public int Count(Allocator allocator)
		{
			return CountId.Value(allocator);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Add(Allocator allocator, T item)
		{
			ref var count = ref CountId.Value(allocator);
			EnsureCapacityAt(allocator, count);
			ItemsId.GetAtUnchecked(allocator, count++) = item;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool Remove(Allocator allocator, T item)
		{
			var index = IndexOf(allocator, item);
			if (index >= 0)
			{
				RemoveAt(allocator, index);
				return true;
			}

			return false;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Insert(Allocator allocator, int index, T item)
		{
			ref var count = ref CountId.Value(allocator);
			AllocatorIndexOutOfRangeException.ThrowIfOutOfRangeInclusive(index, count);

			EnsureCapacityAt(allocator, count);

			ItemsId.CopyToSelf(allocator, index, index + 1, count - index);
			ItemsId.GetAtUnchecked(allocator, index) = item;
			count++;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void RemoveAt(Allocator allocator, int index)
		{
			ref var count = ref CountId.Value(allocator);
			AllocatorIndexOutOfRangeException.ThrowIfOutOfRangeExclusive(index, count);

			count--;
			ItemsId.CopyToSelf(allocator, index + 1, index, count - index);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void RemoveAtSwapBack(Allocator allocator, int index)
		{
			ref var count = ref CountId.Value(allocator);
			AllocatorIndexOutOfRangeException.ThrowIfOutOfRangeExclusive(index, count);

			count--;
			var lastIndex = count;
			ItemsId.GetAtUnchecked(allocator, index) = ItemsId.GetAtUnchecked(allocator, lastIndex);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public int IndexOf(Allocator allocator, T item)
		{
			return ItemsId.IndexOf(allocator, item, 0, Count(allocator));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Clear(Allocator allocator)
		{
			CountId.Value(allocator) = 0;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void EnsureCapacityAt(Allocator allocator, int index)
		{
			ItemsId.EnsureCapacity(allocator, index + 1);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public UnsafeEnumerator<T> Enumerate(Allocator allocator)
		{
			UnsafeEnumerator<T> unsafeEnumerator = default;
			unsafeEnumerator.Data = (T*)(allocator.AlignedPtr + allocator.GetChunk(ItemsId.ChunkId).AlignedOffsetInBytes);
			unsafeEnumerator.Length = Count(allocator);
			unsafeEnumerator.Index = -1;
			return unsafeEnumerator;
		}
	}
}
