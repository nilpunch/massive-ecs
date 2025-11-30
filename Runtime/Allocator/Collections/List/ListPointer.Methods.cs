#if !MASSIVE_DISABLE_ASSERT
#define MASSIVE_ASSERT
#endif

using System;
using System.Runtime.CompilerServices;

namespace Massive
{
	public readonly partial struct ListPointer<T>
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
			return ref Model.Value(allocator)[allocator, index];
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public int Count(Allocator allocator)
		{
			return Model.Value(allocator).Count;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public int Capacity(Allocator allocator)
		{
			return Model.Value(allocator).Capacity;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Add(Allocator allocator, T item)
		{
			Model.Value(allocator).Add(allocator, item);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool Remove<U>(Allocator allocator, U item) where U : IEquatable<T>
		{
			return Model.Value(allocator).Remove(allocator, item);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Insert(Allocator allocator, int index, T item)
		{
			Model.Value(allocator).Insert(allocator, index, item);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void RemoveAt(Allocator allocator, int index)
		{
			Model.Value(allocator).RemoveAt(allocator, index);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void RemoveAtSwapBack(Allocator allocator, int index)
		{
			Model.Value(allocator).RemoveAtSwapBack(allocator, index);
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
		public void Clear(Allocator allocator)
		{
			Model.Value(allocator).Clear();
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public UnsafeEnumerator<T> GetEnumerator(Allocator allocator)
		{
			return Model.Value(allocator).GetEnumerator(allocator);
		}
	}
}
