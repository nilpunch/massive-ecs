#if !MASSIVE_DISABLE_ASSERT
#define MASSIVE_ASSERT
#endif

using System;
using System.Runtime.CompilerServices;
using Unity.IL2CPP.CompilerServices;

namespace Massive
{
	[Il2CppSetOption(Option.NullChecks, false)]
	[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
	public readonly ref struct WorkableArray<T> where T : unmanaged
	{
		public readonly ChunkId ChunkId;
		public readonly Allocator<T> Allocator;

		public WorkableArray(ChunkId chunkId, Allocator<T> allocator)
		{
			// Assert.
			allocator.GetChunk(chunkId);

			ChunkId = chunkId;
			Allocator = allocator;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static implicit operator AllocatorChunkId(WorkableArray<T> array)
		{
			return new AllocatorChunkId(array.ChunkId, array.Allocator.AllocatorId);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static implicit operator ArrayHandle<T>(WorkableArray<T> array)
		{
			return new ArrayHandle<T>(array.ChunkId);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Free()
		{
			Allocator.Free(ChunkId);
		}

		public ref T this[int index]
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				AllocatorIndexOutOfRangeException.ThrowIfOutOfRangeExclusive(index, Allocator.Chunks[ChunkId.Id].Length);

				return ref Allocator.Data[Allocator.Chunks[ChunkId.Id].Offset + index];
			}
		}

		public int Length
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => Allocator.Chunks[ChunkId.Id].Length;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public ref T GetAtUnchecked(int index)
		{
			return ref Allocator.Data[Allocator.Chunks[ChunkId.Id].Offset + index];
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Resize(int minimalLength, MemoryInit memoryInit = MemoryInit.Clear)
		{
			Allocator.Resize(ChunkId, minimalLength, memoryInit);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public int IndexOf(T item)
		{
			return Array.IndexOf(Allocator.Data, item, Allocator.Chunks[ChunkId.Id].Offset, Allocator.Chunks[ChunkId.Id].Length);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public int IndexOf(T item, int startIndex, int count)
		{
			var chunk = Allocator.Chunks[ChunkId.Id];

			if (startIndex + count >= chunk.Offset + chunk.Length)
			{
				return -1;
			}

			return Array.IndexOf(Allocator.Data, item, chunk.Offset + startIndex, count);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void CopyTo(int sourceIndex, WorkableArray<T> destinationArray, int destinationIndex, int length)
		{
			Array.Copy(Allocator.Data, Allocator.Chunks[ChunkId.Id].Offset + sourceIndex,
				destinationArray.Allocator.Data, destinationArray.Allocator.Chunks[ChunkId.Id].Offset + destinationIndex,
				length);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void CopyToSelf(int sourceIndex, int destinationIndex, int length)
		{
			Array.Copy(Allocator.Data, Allocator.Chunks[ChunkId.Id].Offset + sourceIndex,
				Allocator.Data, Allocator.Chunks[ChunkId.Id].Offset + destinationIndex,
				length);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void EnsureCapacity(int capacity)
		{
			if (capacity > Allocator.Chunks[ChunkId.Id].Length)
			{
				Allocator.Resize(ChunkId, capacity);
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Enumerator GetEnumerator()
		{
			return new Enumerator(this);
		}

		public struct Enumerator
		{
			private readonly T[] _data;
			private readonly int _offset;
			private readonly int _length;
			private int _index;

			public Enumerator(WorkableArray<T> list)
			{
				_data = list.Allocator.Data;
				_offset = list.Allocator.Chunks[list.ChunkId.Id].Offset;
				_length = list.Allocator.Chunks[list.ChunkId.Id].Length;
				_index = -1;
			}

			public Enumerator(WorkableArray<T> list, int start, int length)
			{
				_data = list.Allocator.Data;
				_offset = list.Allocator.Chunks[list.ChunkId.Id].Offset + start;
				_length = length;
				_index = -1;
			}

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public bool MoveNext()
			{
				return ++_index < _length;
			}

			public ref T Current
			{
				[MethodImpl(MethodImplOptions.AggressiveInlining)]
				get => ref _data[_offset + _index];
			}
		}
	}
}
