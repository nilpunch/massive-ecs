using System.Runtime.CompilerServices;
using Unity.IL2CPP.CompilerServices;

namespace Massive
{
	[Il2CppSetOption(Option.NullChecks, false)]
	[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
	public readonly struct AutoAllocator<T> where T : unmanaged
	{
		public readonly Allocator<T> Allocator;
		public readonly Allocators Allocators;

		public AutoAllocator(Allocator<T> allocator, Allocators allocators)
		{
			Allocator = allocator;
			Allocators = allocators;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public ChunkId Alloc(int minimumLength, MemoryInit memoryInit = MemoryInit.Clear)
		{
			return Allocator.Alloc(minimumLength, memoryInit);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public ChunkId AllocAuto(int id, int minimumLength, MemoryInit memoryInit = MemoryInit.Clear)
		{
			var chunkId = Allocator.Alloc(minimumLength, memoryInit);
			Allocators.TrackAllocation(id, chunkId, Allocator.AllocatorId);
			return chunkId;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public WorkableVar<T> AllocVar(T value = default)
		{
			var chunkId = Allocator.Alloc(1, MemoryInit.Uninitialized);
			Allocator.Data[Allocator.Chunks[chunkId.Id].Offset] = value;
			return new WorkableVar<T>(chunkId, Allocator);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public WorkableVar<T> AllocAutoVar(int id, T value = default)
		{
			var chunkId = Allocator.Alloc(1, MemoryInit.Uninitialized);
			Allocator.Data[Allocator.Chunks[chunkId.Id].Offset] = value;
			Allocators.TrackAllocation(id, chunkId, Allocator.AllocatorId);
			return new WorkableVar<T>(chunkId, Allocator);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public WorkableArray<T> AllocArray(int minimumLength, MemoryInit memoryInit = MemoryInit.Clear)
		{
			return new WorkableArray<T>(Allocator.Alloc(minimumLength, memoryInit), Allocator);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public WorkableArray<T> AllocAutoArray(int id, int minimumLength, MemoryInit memoryInit = MemoryInit.Clear)
		{
			var chunkId = Allocator.Alloc(minimumLength, memoryInit);
			Allocators.TrackAllocation(id, chunkId, Allocator.AllocatorId);
			return new WorkableArray<T>(chunkId, Allocator);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public WorkableList<T> AllocList(int capacity = 0)
		{
			var items = Allocator.Alloc(capacity, MemoryInit.Uninitialized);

			var countAllocator = Allocators.IntAllocator;
			var count = countAllocator.Alloc(1, MemoryInit.Uninitialized);
			countAllocator.Data[countAllocator.Chunks[count.Id].Offset] = 0;

			return new WorkableList<T>(items, count, Allocator, countAllocator);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public WorkableList<T> AllocAutoList(int id, int capacity = 0)
		{
			var items = Allocator.Alloc(capacity, MemoryInit.Uninitialized);

			var countAllocator = Allocators.IntAllocator;
			var count = countAllocator.Alloc(1, MemoryInit.Uninitialized);
			countAllocator.Data[countAllocator.Chunks[count.Id].Offset] = 0;

			Allocators.TrackAllocation(id, items, Allocator.AllocatorId);
			Allocators.TrackAllocation(id, count, countAllocator.AllocatorId);

			return new WorkableList<T>(items, count, Allocator, countAllocator);
		}
	}
}
