using System;
using System.Runtime.CompilerServices;
using Unity.IL2CPP.CompilerServices;

namespace Massive
{
	[Il2CppSetOption(Option.NullChecks, false)]
	[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
	public readonly struct WorkableChunk<T> where T : unmanaged
	{
		private readonly ChunkId _chunkId;
		private readonly Allocator<T> _allocator;

		public WorkableChunk(ChunkId chunkId, Allocator<T> allocator)
		{
			_chunkId = chunkId;
			_allocator = allocator;
		}

		public ChunkHandle<T> Handle
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => new ChunkHandle<T>(_chunkId);
		}

		public ref T this[int index]
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				if (index >= _allocator.Chunks[_chunkId.Id].Length)
				{
					throw new IndexOutOfRangeException();
				}

				return ref _allocator.Data[_allocator.Chunks[_chunkId.Id].Offset + index];
			}
		}

		public int Length
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => _allocator.Chunks[_chunkId.Id].Length;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Resize(int minimalLength, MemoryInit memoryInit = MemoryInit.Clear)
		{
			_allocator.Resize(_chunkId, minimalLength, memoryInit);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public int IndexOf(T item)
		{
			return Array.IndexOf(_allocator.Data, item, _allocator.Chunks[_chunkId.Id].Offset, _allocator.Chunks[_chunkId.Id].Length);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public int IndexOf(T item, int startIndex, int count)
		{
			if (startIndex + count >= _allocator.Chunks[_chunkId.Id].Offset + _allocator.Chunks[_chunkId.Id].Length)
			{
				return -1;
			}

			return Array.IndexOf(_allocator.Data, item, _allocator.Chunks[_chunkId.Id].Offset + startIndex, count);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void CopyTo(int sourceIndex, WorkableChunk<T> destinationChunk, int destinationIndex, int length)
		{
			Array.Copy(_allocator.Data, _allocator.Chunks[_chunkId.Id].Offset + sourceIndex,
				destinationChunk._allocator.Data, destinationChunk._allocator.Chunks[_chunkId.Id].Offset + destinationIndex,
				length);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void CopyToSelf(int sourceIndex, int destinationIndex, int length)
		{
			Array.Copy(_allocator.Data, _allocator.Chunks[_chunkId.Id].Offset + sourceIndex,
				_allocator.Data, _allocator.Chunks[_chunkId.Id].Offset + destinationIndex,
				length);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void EnsureCapacity(int capacity)
		{
			if (capacity > _allocator.Chunks[_chunkId.Id].Length)
			{
				_allocator.Resize(_chunkId, capacity);
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
				_offset = list._allocator.Chunks[list._chunkId.Id].Offset;
				_length = list._allocator.Chunks[list._chunkId.Id].Length;
				_index = -1;
			}

			public Enumerator(WorkableChunk<T> list, int start, int length)
			{
				_data = list._allocator.Data;
				_offset = list._allocator.Chunks[list._chunkId.Id].Offset + start;
				_length = length;
				_index = -1;
			}

			public bool MoveNext() => ++_index < _length;

			public ref T Current => ref _data[_offset + _index];
		}
	}
}
