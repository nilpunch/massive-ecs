using System;
using System.Runtime.CompilerServices;
using Unity.IL2CPP.CompilerServices;

namespace Massive
{
	[Il2CppSetOption(Option.NullChecks, false)]
	[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
	public readonly unsafe ref struct WorkableChunk<T>
	{
		private readonly ChunkId _chunkId;
		private readonly Chunk* _chunk;
		private readonly Allocator<T> _allocator;

		public WorkableChunk(ChunkId chunkId, Allocator<T> allocator)
		{
			_chunkId = chunkId;

			fixed (Chunk* chunkPtr = &allocator.GetChunk(_chunkId))
			{
				_chunk = chunkPtr;
			}

			_allocator = allocator;
		}

		public ChunkHandle<T> AsHandle => new ChunkHandle<T>(_chunkId);

		public ref T this[int index]
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				if (index >= _chunk->Length)
				{
					throw new IndexOutOfRangeException();
				}

				return ref _allocator.Data[_chunk->Offset + index];
			}
		}

		public int Length
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => _chunk->Length;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Resize(int minimalLength)
		{
			_allocator.Resize(_chunkId, minimalLength);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void CopyTo(int sourceIndex, WorkableChunk<T> destinationChunk, int destinationIndex, int length)
		{
			Array.Copy(_allocator.Data, _chunk->Offset + sourceIndex,
				destinationChunk._allocator.Data, destinationChunk._chunk->Offset + destinationIndex,
				length);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void CopyToSelf(int sourceIndex, int destinationIndex, int length)
		{
			Array.Copy(_allocator.Data, _chunk->Offset + sourceIndex,
				_allocator.Data, _chunk->Offset + destinationIndex,
				length);
		}
		
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void EnsureCapacityAt(int index)
		{
			if (index >= _chunk->Length)
			{
				_allocator.Resize(_chunkId, MathUtils.NextPowerOf2(index + 1));
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
				_offset = list._chunk->Offset;
				_length = list._chunk->Length;
				_index = -1;
			}

			public Enumerator(WorkableChunk<T> list, int start, int length)
			{
				_data = list._allocator.Data;
				_offset = list._chunk->Offset + start;
				_length = length;
				_index = -1;
			}

			public bool MoveNext() => ++_index < _length;

			public ref T Current => ref _data[_offset + _index];
		}
	}
}
