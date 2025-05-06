using System.Runtime.CompilerServices;

namespace Massive
{
	public static class AllocatorCollectionExtensions
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static WorkableChunk<T> AllocChunk<T>(this Allocator<T> allocator)
		{
			return new WorkableChunk<T>(allocator.Alloc(0), allocator);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static WorkableVar<T> AllocVar<T>(this Allocator<T> allocator)
		{
			return new WorkableVar<T>(allocator.Alloc(1), allocator);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static WorkableList<T> AllocList<T>(this ListAllocator<T> allocator)
		{
			return new WorkableList<T>(new ListHandle<T>(
				new ChunkHandle<T>(allocator.Items.Alloc(0)),
				new VarHandle<int>(allocator.Count.Alloc(1))),
				allocator);
		}
	}
}
