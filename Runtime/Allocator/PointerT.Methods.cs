#if !MASSIVE_DISABLE_ASSERT
#define MASSIVE_ASSERT
#endif

using System.Runtime.CompilerServices;

namespace Massive
{
	public unsafe partial struct Pointer<T>
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public readonly void Free(Allocator allocator)
		{
			allocator.Free(AsPointer);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void DeepFree(Allocator allocator)
		{
			AllocatorTypeSchema<T>.DeepFree(allocator, this);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public readonly ref T Value(Allocator allocator)
		{
			return ref *(T*)(allocator.GetPage(AsPointer).AlignedPtr + AsPointer.Offset);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public readonly ref T GetAt(Allocator allocator, int index)
		{
			ref readonly var page = ref allocator.GetPage(AsPointer);

			AllocatorIndexOutOfRangeException.ThrowIfOutOfRangeExclusive(index * Unmanaged<T>.SizeInBytes, 1 << page.SizeClass);

			return ref ((T*)(allocator.GetPage(AsPointer).AlignedPtr + AsPointer.Offset))[index];
		}
	}
}
