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

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public readonly void Free(Allocator allocator)
		{
			allocator.Free(Raw);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public readonly void DeepFree(Allocator allocator)
		{
			AllocatorTypeSchema<T>.DeepFree(allocator, this);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public readonly unsafe ref T Value(Allocator allocator)
		{
			ref readonly var page = ref allocator.GetPage(Raw);

			AllocatorOutOfRangeException.ThrowIfNotFitsInSlot(Unmanaged<T>.SizeInBytes, page.SlotLength);

			return ref *(T*)(page.AlignedPtr + Raw.Offset);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public readonly unsafe ref T GetAt(Allocator allocator, int index)
		{
			ref readonly var page = ref allocator.GetPage(Raw);

			AllocatorOutOfRangeException.ThrowIfOutOfRangeExclusive((index * Unmanaged<T>.SizeInBytes) + Unmanaged<T>.SizeInBytes - 1, page.SlotLength);

			return ref ((T*)(page.AlignedPtr + Raw.Offset))[index];
		}
	}
}
