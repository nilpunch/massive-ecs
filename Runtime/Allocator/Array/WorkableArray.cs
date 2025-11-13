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
		private readonly ArrayPointer<T> _arrayPointer;
		private readonly Allocator _allocator;

		public WorkableArray(ArrayPointer<T> arrayPointer, Allocator allocator)
		{
			_arrayPointer = arrayPointer;
			_allocator = allocator;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static implicit operator ArrayPointer<T>(WorkableArray<T> array)
		{
			return array._arrayPointer;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Free()
		{
			_arrayPointer.Free(_allocator);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void DeepFree()
		{
			_arrayPointer.DeepFree(_allocator);
		}

		public ref T this[int index]
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => ref _arrayPointer.GetAt(_allocator, index);
		}

		public int Length
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => _arrayPointer.Length(_allocator);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Resize(int minimalLength, MemoryInit memoryInit = MemoryInit.Clear)
		{
			_arrayPointer.Resize(_allocator, minimalLength, memoryInit);
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
			//
			// var chunk = _allocator.GetChunk(_arrayHandle);
			//
			// if ((startIndex + count) * Unmanaged<T>.SizeInBytes >= chunk.OffsetInBytes + chunk.LengthInBytes)
			// {
			// 	return -1;
			// }
			//
			// return Array.IndexOf(_allocator.Data, item, chunk.OffsetInBytes + startIndex, count);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void CopyTo(int sourceIndex, WorkableArray<T> destinationArray, int destinationIndex, int length)
		{
			throw new NotImplementedException();
			// Array.Copy(_allocator.Data, _allocator.GetChunk(_arrayHandle).OffsetInBytes + sourceIndex * Unmanaged<T>.SizeInBytes,
			// 	destinationArray._allocator.Data, destinationArray._allocator.GetChunk(_arrayHandle).OffsetInBytes + destinationIndex * Unmanaged<T>.SizeInBytes,
			// 	length);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void CopyToSelf(int sourceIndex, int destinationIndex, int length)
		{
			_arrayPointer.CopyToSelf(_allocator, sourceIndex, destinationIndex, length);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void EnsureCapacity(int capacity)
		{
			_arrayPointer.EnsureCapacity(_allocator, capacity);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public UnsafeEnumerator<T> GetEnumerator()
		{
			return _arrayPointer.GetEnumerator(_allocator);
		}
	}
}
