using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Massive
{
	public static class AllocatorCollectionExtensions
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static WorkableChunk<T> AllocChunk<T>(this Allocator<T> allocator, int length = 0)
		{
			return new WorkableChunk<T>(allocator.Alloc(length), allocator);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static WorkableVar<T> AllocVar<T>(this Allocator<T> allocator)
		{
			return new WorkableVar<T>(allocator.Alloc(1), allocator);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static WorkableList<T> AllocList<T>(this ListAllocator<T> allocator, int capacity = 0)
		{
			return new WorkableList<T>(new ListHandle<T>(
				new ChunkHandle<T>(allocator.Items.Alloc(capacity)),
				new VarHandle<int>(allocator.Count.Alloc(1))),
				allocator);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void Free<T>(this ListAllocator<T> allocator, ListHandle<T> handle)
		{
			allocator.Items.Free(handle.Items.ChunkId);
			allocator.Count.Free(handle.Count.ChunkId);
		}
	}
}
