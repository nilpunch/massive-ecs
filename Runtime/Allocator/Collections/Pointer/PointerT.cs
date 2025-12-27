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
	[StructLayout(LayoutKind.Sequential, Size = Pointer.Size, Pack = Pointer.Alignment)]
	public struct Pointer<T> where T : unmanaged
	{
		public Pointer Raw;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public WorkablePointer<T> In(Allocator allocator)
		{
			return new WorkablePointer<T>(this, allocator);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static unsafe explicit operator Pointer<T>(Pointer pointer)
		{
			return *(Pointer<T>*)(&pointer);
		}

		/// <summary>
		/// Frees the pointer. See <see cref="DeepFree"/> if you need to deep free the nested pointers.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public readonly void Free(Allocator allocator)
		{
			allocator.Free(Raw);
		}

		/// <summary>
		/// Frees the pointer and all nested allocations.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public readonly void DeepFree(Allocator allocator)
		{
			AllocatorTypeSchema<T>.DeepFree(allocator, this);
		}

		/// <summary>
		/// Returns a reference to the stored value.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public readonly unsafe ref T Value(Allocator allocator)
		{
			ref readonly var page = ref allocator.GetPage(Raw);

			AllocatorOutOfRangeException.ThrowIfNotFitsInSlot(Unmanaged<T>.SizeInBytes, page.SlotLength);

			return ref *(T*)(page.AlignedPtr + Raw.Offset);
		}

		/// <summary>
		/// Returns a reference to an element at the specified index.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public readonly unsafe ref T GetAt(Allocator allocator, int index)
		{
			ref readonly var page = ref allocator.GetPage(Raw);

			AllocatorOutOfRangeException.ThrowIfOutOfRangeExclusive((index * Unmanaged<T>.SizeInBytes) + Unmanaged<T>.SizeInBytes - 1, page.SlotLength);

			return ref ((T*)(page.AlignedPtr + Raw.Offset))[index];
		}

		/// <summary>
		/// Adds an offset to a pointer, using the size of type.
		/// </summary>
		public readonly unsafe Pointer<T> Add(Allocator allocator, int offset)
		{
			var info = Unmanaged<T>.Info;

			var raw = Raw;

#if MASSIVE_ASSERT
			AllocatorOutOfRangeException.ThrowIfOutOfRangeExclusive(
				raw.Offset + (offset * Unmanaged<T>.SizeInBytes) + Unmanaged<T>.SizeInBytes - 1,
				allocator.GetPage(raw).SlotLength);
#endif

			raw.Offset = (ushort)(raw.Offset + offset * info.Size);

			return *(Pointer<T>*)(&raw);
		}
	}
}
