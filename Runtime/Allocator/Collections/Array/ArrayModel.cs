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
	public struct ArrayModel<T> where T : unmanaged
	{
		public const int Size = Pointer.Size + sizeof(int);
		public const int Alignment = Size;

		[AllocatorPointerField(CountFieldName = nameof(Length))]
		public Pointer<T> Items;

		public int Length;

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

				AllocatorOutOfRangeException.ThrowIfOutOfRangeExclusive(index, Length);

				return ref ((T*)(page.AlignedPtr + Items.Raw.Offset))[index];
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public readonly unsafe ref T GetAt(Allocator allocator, int index)
		{
			ref readonly var page = ref allocator.GetPage(Items.Raw);

			AllocatorOutOfRangeException.ThrowIfOutOfRangeExclusive(index, Length);

			return ref ((T*)(page.AlignedPtr + Items.Raw.Offset))[index];
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public readonly ArrayModel<T> Slice(Allocator allocator, int start, int length)
		{
			AllocatorOutOfRangeException.ThrowIfOutOfRangeExclusive(start, Length);
			AllocatorOutOfRangeException.ThrowIfOutOfRangeExclusive(start + length - 1, Length);

			ArrayModel<T> slice = default;
			slice.Items = Items.Add(allocator, start);
			slice.Length = length;
			return slice;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public readonly ArrayModel<T> SliceFrom(Allocator allocator, int start)
		{
			AllocatorOutOfRangeException.ThrowIfOutOfRangeExclusive(start, Length);

			ArrayModel<T> slice = default;
			slice.Items = Items.Add(allocator, start);
			slice.Length = Length - start;
			return slice;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Resize(Allocator allocator, int length, MemoryInit memoryInit = MemoryInit.Clear)
		{
			var info = Unmanaged<T>.Info;
			allocator.Resize(ref Items.Raw, length * info.Size, info.Alignment, memoryInit);
			Length = length;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void EnsureLength(Allocator allocator, int length)
		{
			if (length > Length)
			{
				var info = Unmanaged<T>.Info;
				allocator.Resize(ref Items.Raw, length * info.Size, info.Alignment);
				Length = length;
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public readonly unsafe int IndexOf<U>(Allocator allocator, U item) where U : IEquatable<T>
		{
			var data = (T*)allocator.GetPtr(Items.Raw);
			var endIndex = Length;

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

			AllocatorOutOfRangeException.ThrowIfOutOfRangeExclusive(startIndex, Length);
			AllocatorOutOfRangeException.ThrowIfOutOfRangeInclusive(endIndex, Length);

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
		public readonly unsafe void CopyToSelf(Allocator allocator, int sourceIndex, int destinationIndex, int length)
		{
			var lengthInBytes = length * Unmanaged<T>.SizeInBytes;
			var dataPtr = (T*)allocator.GetPtr(Items.Raw);
			UnsafeUtils.Copy(dataPtr + sourceIndex, dataPtr + destinationIndex, lengthInBytes);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public readonly unsafe UnsafeEnumerator<T> GetEnumerator(Allocator allocator)
		{
			UnsafeEnumerator<T> unsafeEnumerator = default;
			unsafeEnumerator.Data = (T*)allocator.GetPtr(Items.Raw);
			unsafeEnumerator.Length = Length;
			unsafeEnumerator.Index = -1;
			return unsafeEnumerator;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public readonly unsafe UnsafeEnumerator<T> GetEnumerator(Allocator allocator, int start, int length)
		{
			UnsafeEnumerator<T> unsafeEnumerator = default;
			unsafeEnumerator.Data = (T*)allocator.GetPtr(Items.Raw);
			unsafeEnumerator.Length = length;
			unsafeEnumerator.Index = start - 1;
			return unsafeEnumerator;
		}
	}
}
