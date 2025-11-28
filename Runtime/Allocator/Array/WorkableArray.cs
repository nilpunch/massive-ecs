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
			get => ref _arrayPointer.Model.Value(_allocator).GetAt(_allocator, index);
		}

		public int Length
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => _arrayPointer.Model.Value(_allocator).Length;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Resize(int minimalLength, MemoryInit memoryInit = MemoryInit.Clear)
		{
			_arrayPointer.Model.Value(_allocator).Resize(_allocator, minimalLength, memoryInit);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public int IndexOf<U>(U item) where U : IEquatable<T>
		{
			return _arrayPointer.Model.Value(_allocator).IndexOf(_allocator, item);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public int IndexOf<U>(U item, int startIndex, int count) where U : IEquatable<T>
		{
			return _arrayPointer.Model.Value(_allocator).IndexOf(_allocator, item, startIndex, count);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void CopyToSelf(int sourceIndex, int destinationIndex, int length)
		{
			_arrayPointer.Model.Value(_allocator).CopyToSelf(_allocator, sourceIndex, destinationIndex, length);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void EnsureCapacity(int capacity)
		{
			_arrayPointer.Model.Value(_allocator).EnsureLength(_allocator, capacity);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public UnsafeEnumerator<T> GetEnumerator()
		{
			return _arrayPointer.Model.Value(_allocator).GetEnumerator(_allocator);
		}
	}
}
