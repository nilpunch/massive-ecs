using System.Runtime.CompilerServices;

namespace Massive
{
	public readonly unsafe partial struct VarHandle<T>
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Free(Allocator allocator)
		{
			allocator.Free(Pointer.AsPointer);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void DeepFree(Allocator allocator)
		{
			AllocatorTypeSchema<T>.DeepFree(allocator, Pointer);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public ref T Value(Allocator allocator)
		{
			return ref *(T*)(allocator.GetPage(Pointer.AsPointer).AlignedPtr + Pointer.AsPointer.Offset);
		}
	}
}
