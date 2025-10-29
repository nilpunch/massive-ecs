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
	public readonly struct WorkableArray<T> where T : unmanaged
	{
		private readonly ArrayHandle<T> _arrayHandle;
		private readonly Allocator _allocator;

		public WorkableArray(ArrayHandle<T> arrayHandle, Allocator allocator)
		{
			_arrayHandle = arrayHandle;
			_allocator = allocator;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static implicit operator ArrayHandle<T>(WorkableArray<T> array)
		{
			return new ArrayHandle<T>(array._arrayHandle);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Free()
		{
			_arrayHandle.Free(_allocator);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public WorkableArray<T> Track(int id)
		{
			_arrayHandle.Track(_allocator, id);
			return this;
		}

		public ref T this[int index]
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => ref _arrayHandle.GetAt(_allocator, index);
		}

		public int Length
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => _arrayHandle.Length(_allocator);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public ref T GetAtUnchecked(int index)
		{
			return ref _arrayHandle.GetAtUnchecked(_allocator, index);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Resize(int minimalLength, MemoryInit memoryInit = MemoryInit.Clear)
		{
			_allocator.Resize<T>(_arrayHandle, minimalLength, memoryInit);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public int IndexOf(T item)
		{
			throw new NotImplementedException();
			// return Array.IndexOf(Allocator.Data, item, Allocator.GetChunk(ChunkId).AlignedOffsetInBytes, Allocator.GetChunk(ChunkId).LengthInBytes);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public int IndexOf(T item, int startIndex, int count)
		{
			throw new NotImplementedException();

			var chunk = _allocator.GetChunk(_arrayHandle);

			if ((startIndex + count) * Unmanaged<T>.SizeInBytes >= chunk.OffsetInBytes + chunk.LengthInBytes)
			{
				return -1;
			}

			return Array.IndexOf(_allocator.Data, item, chunk.OffsetInBytes + startIndex, count);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void CopyTo(int sourceIndex, WorkableArray<T> destinationArray, int destinationIndex, int length)
		{
			throw new NotImplementedException();
			Array.Copy(_allocator.Data, _allocator.GetChunk(_arrayHandle).OffsetInBytes + sourceIndex * Unmanaged<T>.SizeInBytes,
				destinationArray._allocator.Data, destinationArray._allocator.GetChunk(_arrayHandle).OffsetInBytes + destinationIndex * Unmanaged<T>.SizeInBytes,
				length);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void CopyToSelf(int sourceIndex, int destinationIndex, int length)
		{
			_arrayHandle.CopyToSelf(_allocator, sourceIndex, destinationIndex, length);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void EnsureCapacity(int capacity)
		{
			_arrayHandle.EnsureCapacity(_allocator, capacity);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public UnsafeEnumerator<T> GetEnumerator()
		{
			return _arrayHandle.GetEnumerator(_allocator);
		}
	}
}
