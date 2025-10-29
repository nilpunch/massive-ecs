using System;
using System.Runtime.CompilerServices;

namespace Massive
{
	public readonly unsafe partial struct ArrayHandle<T>
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Free(Allocator allocator)
		{
			allocator.Free(ChunkId);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public ArrayHandle<T> Track(Allocator allocator, int id)
		{
			allocator.Track(id, ChunkId);
			return this;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public ref T GetAt(Allocator allocator, int index)
		{
			ref var chunk = ref allocator.GetChunk(ChunkId);

			AllocatorIndexOutOfRangeException.ThrowIfOutOfRangeExclusive(index * Unmanaged<T>.SizeInBytes, chunk.LengthInBytes);

			return ref ((T*)(allocator.AlignedPtr + chunk.AlignedOffsetInBytes))[index];
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public ref T GetAtUnchecked(Allocator allocator, int index)
		{
			return ref ((T*)(allocator.AlignedPtr + allocator.GetChunk(ChunkId).AlignedOffsetInBytes))[index];
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public int Length(Allocator allocator)
		{
			var info = Unmanaged<T>.Info;
			return (allocator.GetChunk(ChunkId).LengthInBytes - info.Alignment) / info.Size;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Resize(Allocator allocator, int minimalLength, MemoryInit memoryInit = MemoryInit.Clear)
		{
			allocator.Resize<T>(ChunkId, minimalLength, memoryInit);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public int IndexOf(Allocator allocator, T item)
		{
			throw new NotImplementedException();
			// return Array.IndexOf(Allocator.Data, item, Allocator.GetChunk(ChunkId).AlignedOffsetInBytes, Allocator.GetChunk(ChunkId).LengthInBytes);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public int IndexOf(Allocator allocator, T item, int startIndex, int count)
		{
			throw new NotImplementedException();

			// var chunk = Allocator.GetChunk(ChunkId);
			//
			// if ((startIndex + count) * Unmanaged<T>.SizeInBytes >= chunk.OffsetInBytes + chunk.LengthInBytes)
			// {
			// 	return -1;
			// }
			//
			// return Array.IndexOf(Allocator.Data, item, chunk.OffsetInBytes + startIndex, count);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void CopyTo(Allocator allocator, int sourceIndex, ArrayHandle<T> destinationArray, int destinationIndex, int length)
		{
			throw new NotImplementedException();
			// Array.Copy(Allocator.Data, Allocator.GetChunk(ChunkId).OffsetInBytes + sourceIndex * Unmanaged<T>.SizeInBytes,
			// 	destinationArray.Allocator.Data, destinationArray.Allocator.GetChunk(ChunkId).OffsetInBytes + destinationIndex * Unmanaged<T>.SizeInBytes,
			// 	length);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void CopyToSelf(Allocator allocator, int sourceIndex, int destinationIndex, int length)
		{
			var lengthInBytes = length * Unmanaged<T>.SizeInBytes;
			var alignedOffset = allocator.GetChunk(ChunkId).AlignedOffsetInBytes;
			var alignedChunkPtr = (T*)(allocator.AlignedPtr + alignedOffset);
			Buffer.MemoryCopy(alignedChunkPtr + sourceIndex, alignedChunkPtr + destinationIndex, lengthInBytes, lengthInBytes);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void EnsureCapacity(Allocator allocator, int capacity)
		{
			if (capacity > Length(allocator))
			{
				allocator.Resize<T>(ChunkId, capacity);
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public UnsafeEnumerator<T> GetEnumerator(Allocator allocator)
		{
			UnsafeEnumerator<T> unsafeEnumerator = default;
			unsafeEnumerator.Data = (T*)(allocator.AlignedPtr + allocator.GetChunk(ChunkId).AlignedOffsetInBytes);
			unsafeEnumerator.Length = Length(allocator);
			unsafeEnumerator.Index = -1;
			return unsafeEnumerator;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public UnsafeEnumerator<T> GetEnumerator(Allocator allocator, int length)
		{
			UnsafeEnumerator<T> unsafeEnumerator = default;
			unsafeEnumerator.Data = (T*)(allocator.AlignedPtr + allocator.GetChunk(ChunkId).AlignedOffsetInBytes);
			unsafeEnumerator.Length = length;
			unsafeEnumerator.Index = -1;
			return unsafeEnumerator;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public UnsafeEnumerator<T> GetEnumerator(Allocator allocator, int start, int length)
		{
			UnsafeEnumerator<T> unsafeEnumerator = default;
			unsafeEnumerator.Data = (T*)(allocator.AlignedPtr + allocator.GetChunk(ChunkId).AlignedOffsetInBytes);
			unsafeEnumerator.Length = length;
			unsafeEnumerator.Index = start - 1;
			return unsafeEnumerator;
		}
	}
}
