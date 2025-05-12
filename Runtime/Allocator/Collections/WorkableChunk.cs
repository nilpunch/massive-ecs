using System;
using System.Runtime.CompilerServices;
using Unity.IL2CPP.CompilerServices;

namespace Massive
{
	[Il2CppSetOption(Option.NullChecks, false)]
	[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
	public readonly struct WorkableChunk<T> where T : unmanaged
	{
		public readonly ChunkId ChunkId;

		private readonly Allocator<T> _allocator;

		public WorkableChunk(ChunkId chunkId, Allocator<T> allocator)
		{
			// Assert.
			allocator.GetChunk(chunkId);

			ChunkId = chunkId;
			_allocator = allocator;
		}

		public ChunkHandle<T> Handle
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => new ChunkHandle<T>(ChunkId);
		}

		public ref T this[int index]
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				if (index >= _allocator.Chunks[ChunkId.Id].Length)
				{
					throw new IndexOutOfRangeException();
				}

				return ref _allocator.Data[_allocator.Chunks[ChunkId.Id].Offset + index];
			}
		}

		public int Length
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => _allocator.Chunks[ChunkId.Id].Length;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Resize(int minimalLength, MemoryInit memoryInit = MemoryInit.Clear)
		{
			_allocator.Resize(ChunkId, minimalLength, memoryInit);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public int IndexOf(T item)
		{
			return Array.IndexOf(_allocator.Data, item, _allocator.Chunks[ChunkId.Id].Offset, _allocator.Chunks[ChunkId.Id].Length);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public int IndexOf(T item, int startIndex, int count)
		{
			if (startIndex + count >= _allocator.Chunks[ChunkId.Id].Offset + _allocator.Chunks[ChunkId.Id].Length)
			{
				return -1;
			}

			return Array.IndexOf(_allocator.Data, item, _allocator.Chunks[ChunkId.Id].Offset + startIndex, count);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void CopyTo(int sourceIndex, WorkableChunk<T> destinationChunk, int destinationIndex, int length)
		{
			Array.Copy(_allocator.Data, _allocator.Chunks[ChunkId.Id].Offset + sourceIndex,
				destinationChunk._allocator.Data, destinationChunk._allocator.Chunks[ChunkId.Id].Offset + destinationIndex,
				length);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void CopyToSelf(int sourceIndex, int destinationIndex, int length)
		{
			Array.Copy(_allocator.Data, _allocator.Chunks[ChunkId.Id].Offset + sourceIndex,
				_allocator.Data, _allocator.Chunks[ChunkId.Id].Offset + destinationIndex,
				length);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void EnsureCapacity(int capacity)
		{
			if (capacity > _allocator.Chunks[ChunkId.Id].Length)
			{
				_allocator.Resize(ChunkId, capacity);
			}
		}

		public Enumerator GetEnumerator() => new Enumerator(this);

		public struct Enumerator
		{
			private readonly T[] _data;
			private readonly int _offset;
			private readonly int _length;
			private int _index;

			public Enumerator(WorkableChunk<T> list)
			{
				_data = list._allocator.Data;
				_offset = list._allocator.Chunks[list.ChunkId.Id].Offset;
				_length = list._allocator.Chunks[list.ChunkId.Id].Length;
				_index = -1;
			}

			public Enumerator(WorkableChunk<T> list, int start, int length)
			{
				_data = list._allocator.Data;
				_offset = list._allocator.Chunks[list.ChunkId.Id].Offset + start;
				_length = length;
				_index = -1;
			}

			public bool MoveNext() => ++_index < _length;

			public ref T Current => ref _data[_offset + _index];
		}
	}
}
