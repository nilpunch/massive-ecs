#if !MASSIVE_DISABLE_ASSERT
#define MASSIVE_ASSERT
#endif

using System;
using System.Runtime.CompilerServices;

namespace Massive
{
	public partial struct ArrayPointer<T>
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Free(Allocator allocator)
		{
			ref var model = ref Model.Value(allocator);
			allocator.Free(model.Items.Raw);
			allocator.Free(Model.Raw);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void DeepFree(Allocator allocator)
		{
			Model.DeepFree(allocator);
		}

		public ref T this[Allocator allocator, int index]
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => ref Model.Value(allocator)[allocator, index];
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public ref T GetAt(Allocator allocator, int index)
		{
			return ref Model.Value(allocator).GetAt(allocator, index);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public int Length(Allocator allocator)
		{
			return Model.Value(allocator).Length;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Resize(Allocator allocator, int length, MemoryInit memoryInit = MemoryInit.Clear)
		{
			Model.Value(allocator).Resize(allocator, length, memoryInit);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public int IndexOf<U>(Allocator allocator, U item) where U : IEquatable<T>
		{
			return Model.Value(allocator).IndexOf(allocator, item);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public int IndexOf<U>(Allocator allocator, U item, int startIndex, int count) where U : IEquatable<T>
		{
			return Model.Value(allocator).IndexOf(allocator, item, startIndex, count);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void CopyToSelf(Allocator allocator, int sourceIndex, int destinationIndex, int length)
		{
			Model.Value(allocator).CopyToSelf(allocator, sourceIndex, destinationIndex, length);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void EnsureLength(Allocator allocator, int length)
		{
			Model.Value(allocator).EnsureLength(allocator, length);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public UnsafeEnumerator<T> GetEnumerator(Allocator allocator)
		{
			return Model.Value(allocator).GetEnumerator(allocator);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public UnsafeEnumerator<T> GetEnumerator(Allocator allocator, int start, int length)
		{
			return Model.Value(allocator).GetEnumerator(allocator, start, length);
		}
	}
}
