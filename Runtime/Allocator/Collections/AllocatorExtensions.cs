using System.Runtime.CompilerServices;
using Unity.IL2CPP.CompilerServices;

namespace Massive
{
	[Il2CppSetOption(Option.NullChecks, false)]
	[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
	public static class AllocatorExtensions
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static WorkableVar<T> AllocVar<T>(this Allocator<T> allocator) where T : unmanaged
		{
			return new WorkableVar<T>(allocator.Alloc(1), allocator);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static WorkableChunk<T> AllocChunk<T>(this Allocator<T> allocator, int length = 0) where T : unmanaged
		{
			return new WorkableChunk<T>(allocator.Alloc(length), allocator);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static WorkableList<T> AllocList<T>(this ListAllocator<T> allocator, int capacity = 0) where T : unmanaged
		{
			var count = allocator.Count.Alloc(1, MemoryInit.Uninitialized);
			allocator.Count.Data[allocator.Count.Chunks[count.Id].Offset] = 0;

			return new WorkableList<T>(
				allocator.Items.Alloc(capacity, MemoryInit.Uninitialized),
				count,
				allocator);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void Free<T>(this Allocator<T> allocator, VarHandle<T> handle) where T : unmanaged
		{
			allocator.Free(handle.ChunkId);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void Free<T>(this Allocator<T> allocator, ChunkHandle<T> handle) where T : unmanaged
		{
			allocator.Free(handle.ChunkId);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void Free<T>(this ListAllocator<T> allocator, ListHandle<T> handle) where T : unmanaged
		{
			allocator.Items.Free(handle.Items);
			allocator.Count.Free(handle.Count);
		}
	}
}
