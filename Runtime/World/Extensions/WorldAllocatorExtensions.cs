using System.Runtime.CompilerServices;
using Unity.IL2CPP.CompilerServices;

namespace Massive
{
	[Il2CppSetOption(Option.NullChecks, false)]
	[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
	public static class WorldAllocatorExtensions
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Allocator<T> Allocator<T>(this World world) where T : unmanaged
		{
			var allocatorId = AllocatorId<T>.Index;
			var allocators = world.Allocators;

			allocators.EnsureLookupAt(allocatorId);
			var candidate = allocators.Lookup[allocatorId];

			if (candidate != null)
			{
				return (Allocator<T>)candidate;
			}

			var allocator = new Allocator<T>(DefaultValueUtils.GetDefaultValueFor<T>());
			var cloner = new AllocatorCloner<T>(allocator);

			allocators.Insert(AllocatorId<T>.FullName, allocator, cloner);
			allocators.Lookup[allocatorId] = allocator;

			return allocator;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static AutoAllocator<T> AutoAllocator<T>(this World world) where T : unmanaged
		{
			var allocatorId = AllocatorId<T>.Index;
			var allocatorRegistry = world.Allocators;

			allocatorRegistry.EnsureLookupAt(allocatorId);
			var candidate = allocatorRegistry.Lookup[allocatorId];

			if (candidate != null)
			{
				return new AutoAllocator<T>((Allocator<T>)candidate, allocatorRegistry);
			}

			var allocator = new Allocator<T>(DefaultValueUtils.GetDefaultValueFor<T>());
			var cloner = new AllocatorCloner<T>(allocator);

			allocatorRegistry.Insert(AllocatorId<T>.FullName, allocator, cloner);
			allocatorRegistry.Lookup[allocatorId] = allocator;

			return new AutoAllocator<T>(allocator, allocatorRegistry);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static WorkableVar<T> AllocVar<T>(this World world, T value = default) where T : unmanaged
		{
			return world.AutoAllocator<T>().AllocVar(value);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static WorkableVar<T> AllocAutoVar<T>(this World world, int id, T value = default) where T : unmanaged
		{
			return world.AutoAllocator<T>().AllocAutoVar(id, value);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static WorkableArray<T> AllocArray<T>(this World world, int minimumLength, MemoryInit memoryInit = MemoryInit.Clear) where T : unmanaged
		{
			return world.AutoAllocator<T>().AllocArray(minimumLength, memoryInit);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static WorkableArray<T> AllocAutoArray<T>(this World world, int id, int minimumLength, MemoryInit memoryInit = MemoryInit.Clear) where T : unmanaged
		{
			return world.AutoAllocator<T>().AllocAutoArray(id, minimumLength, memoryInit);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static WorkableList<T> AllocList<T>(this World world, int capacity = 0) where T : unmanaged
		{
			return world.AutoAllocator<T>().AllocList(capacity);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static WorkableList<T> AllocAutoList<T>(this World world, int id, int capacity = 0) where T : unmanaged
		{
			return world.AutoAllocator<T>().AllocAutoList(id, capacity);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void Free(this World world, AllocatorChunkId allocatorChunkId)
		{
			world.Allocators.Lookup[allocatorChunkId.AllocatorId].Free(allocatorChunkId.ChunkId);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void Free(this World world, AllocatorListId allocatorListId)
		{
			world.Allocators.Lookup[allocatorListId.ItemsId].Free(allocatorListId.Items);
			world.Allocators.Lookup[world.Allocators.IntId].Free(allocatorListId.Count);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void AutoFree(this World world, int id, AllocatorChunkId allocatorChunkId)
		{
			world.Allocators.TrackAllocation(id, allocatorChunkId.ChunkId, allocatorChunkId.AllocatorId);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void AutoFree(this World world, int id, AllocatorListId allocatorListId)
		{
			world.Allocators.TrackAllocation(id, allocatorListId.Items, allocatorListId.ItemsId);
			world.Allocators.TrackAllocation(id, allocatorListId.Count, world.Allocators.IntId);
		}
	}
}
