#if !MASSIVE_DISABLE_ASSERT
#define MASSIVE_ASSERT
#endif

using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Unity.IL2CPP.CompilerServices;

namespace Massive
{
	[Il2CppSetOption(Option.NullChecks, false)]
	[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
	[StructLayout(LayoutKind.Sequential, Size = Size, Pack = Alignment)]
	public struct ListModel<T> where T : unmanaged
	{
		public const int Size = Pointer.Size + sizeof(int) * 2;
		public const int Alignment = 16;

		[AllocatorPointerField(CountFieldName = nameof(Count))]
		public Pointer<T> Items;

		public int Capacity;
		public int Count;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public readonly void Free(Allocator allocator)
		{
			Items.Free(allocator);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public readonly void DeepFree(Allocator allocator)
		{
			Items.DeepFree(allocator);
		}

		public readonly unsafe ref T this[Allocator allocator, int index]
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				ref readonly var page = ref allocator.GetPage(Items.Raw);

				AllocatorOutOfRangeException.ThrowIfOutOfRangeExclusive(index, Count);

				return ref ((T*)(page.AlignedPtr + Items.Raw.Offset))[index];
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public readonly unsafe ref T GetAt(Allocator allocator, int index)
		{
			ref readonly var page = ref allocator.GetPage(Items.Raw);

			AllocatorOutOfRangeException.ThrowIfOutOfRangeExclusive(index, Count);

			return ref ((T*)(page.AlignedPtr + Items.Raw.Offset))[index];
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe void Add(Allocator allocator, T item)
		{
			if (Count >= Capacity)
			{
				var newCapacity = MathUtils.RoundUpToPowerOfTwo(Count + 1);

				var info = Unmanaged<T>.Info;
				allocator.Resize(ref Items.Raw, newCapacity * info.Size, info.Alignment);
				Capacity = newCapacity;
			}

			((T*)allocator.GetPtr(Items.Raw))[Count++] = item;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe bool Remove<U>(Allocator allocator, U item) where U : IEquatable<T>
		{
			var data = (T*)allocator.GetPtr(Items.Raw);
			var endIndex = Count;

			for (var i = 0; i < endIndex; i++)
			{
				if (item.Equals(data[i]))
				{
					RemoveAt(allocator, i);
					return true;
				}
			}

			return false;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe void Insert(Allocator allocator, int index, T item)
		{
			AllocatorOutOfRangeException.ThrowIfOutOfRangeInclusive(index, Count);

			EnsureCapacityAt(allocator, Count);

			CopyToSelf(allocator, index, index + 1, Count - index);
			((T*)allocator.GetPtr(Items.Raw))[index] = item;
			Count++;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void RemoveAt(Allocator allocator, int index)
		{
			AllocatorOutOfRangeException.ThrowIfOutOfRangeExclusive(index, Count);

			Count--;
			CopyToSelf(allocator, index + 1, index, Count - index);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe void RemoveAtSwapBack(Allocator allocator, int index)
		{
			AllocatorOutOfRangeException.ThrowIfOutOfRangeExclusive(index, Count);

			Count--;

			var items = (T*)allocator.GetPtr(Items.Raw);
			items[index] = items[Count];
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public readonly unsafe int IndexOf<U>(Allocator allocator, U item) where U : IEquatable<T>
		{
			var data = (T*)allocator.GetPtr(Items.Raw);
			var endIndex = Count;

			for (var i = 0; i < endIndex; i++)
			{
				if (item.Equals(data[i]))
				{
					return i;
				}
			}
			return -1;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public readonly unsafe int IndexOf<U>(Allocator allocator, U item, int startIndex, int count) where U : IEquatable<T>
		{
			var data = (T*)allocator.GetPtr(Items.Raw);
			var endIndex = startIndex + count;

			AllocatorOutOfRangeException.ThrowIfOutOfRangeExclusive(startIndex, Count);
			AllocatorOutOfRangeException.ThrowIfOutOfRangeInclusive(endIndex, Count);

			for (var i = startIndex; i < endIndex; i++)
			{
				if (item.Equals(data[i]))
				{
					return i;
				}
			}
			return -1;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Clear()
		{
			Count = 0;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void EnsureCapacityAt(Allocator allocator, int index)
		{
			if (index >= Capacity)
			{
				var newCapacity = MathUtils.RoundUpToPowerOfTwo(index + 1);

				var info = Unmanaged<T>.Info;
				allocator.Resize(ref Items.Raw, newCapacity * info.Size, info.Alignment);
				Capacity = newCapacity;
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public readonly unsafe UnsafeEnumerator<T> GetEnumerator(Allocator allocator)
		{
			UnsafeEnumerator<T> unsafeEnumerator = default;
			unsafeEnumerator.Data = (T*)allocator.GetPtr(Items.Raw);
			unsafeEnumerator.Length = Count;
			unsafeEnumerator.Index = -1;
			return unsafeEnumerator;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private readonly unsafe void CopyToSelf(Allocator allocator, int sourceIndex, int destinationIndex, int length)
		{
			var lengthInBytes = length * Unmanaged<T>.SizeInBytes;
			var alignedChunkPtr = (T*)allocator.GetPtr(Items.Raw);
			UnsafeUtils.Copy(alignedChunkPtr + sourceIndex, alignedChunkPtr + destinationIndex, lengthInBytes);
		}
	}
}
