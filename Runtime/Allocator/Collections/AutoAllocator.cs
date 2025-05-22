using System.Runtime.CompilerServices;
using Unity.IL2CPP.CompilerServices;

namespace Massive
{
	[Il2CppSetOption(Option.NullChecks, false)]
	[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
	public struct AutoAllocator<T> where T : unmanaged
	{
		public Allocator<T> Allocator;
		public Allocators Registry;
		public int AllocatorId;

		public AutoAllocator(Allocator<T> allocator, Allocators registry)
		{
			Allocator = allocator;
			Registry = registry;
			AllocatorId = AllocatorId<T>.Index;
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
			Registry.TrackAllocation(id, chunkId, AllocatorId);
			return chunkId;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public WorkableVar<T> AllocVar(T value = default)
		{
			var chunkId = Allocator.Alloc(1);
			Allocator.Data[Allocator.Chunks[chunkId.Id].Offset] = value;
			return new WorkableVar<T>(chunkId, Allocator);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public WorkableVar<T> AllocAutoVar(int id, T value = default)
		{
			var chunkId = Allocator.Alloc(1);
			Allocator.Data[Allocator.Chunks[chunkId.Id].Offset] = value;
			Registry.TrackAllocation(id, chunkId, AllocatorId);
			return new WorkableVar<T>(chunkId, Allocator);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public WorkableChunk<T> AllocChunk(int minimumLength, MemoryInit memoryInit = MemoryInit.Clear)
		{
			return new WorkableChunk<T>(Allocator.Alloc(minimumLength, memoryInit), Allocator);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public WorkableChunk<T> AllocAutoChunk(int id, int minimumLength, MemoryInit memoryInit = MemoryInit.Clear)
		{
			var chunkId = Allocator.Alloc(minimumLength, memoryInit);
			Registry.TrackAllocation(id, chunkId, AllocatorId);
			return new WorkableChunk<T>(chunkId, Allocator);
		}
	}
}
