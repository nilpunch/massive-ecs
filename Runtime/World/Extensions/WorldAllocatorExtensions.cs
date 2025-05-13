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
			var info = AllocatorTypeId<T>.Info;
			var allocatorRegistry = world.AllocatorRegistry;

			allocatorRegistry.EnsureLookupAt(info.Index);
			var candidate = allocatorRegistry.Lookup[info.Index];

			if (candidate != null)
			{
				return (Allocator<T>)candidate;
			}

			var allocator = new Allocator<T>(DefaultValueUtils.GetDefaultValueFor<T>());
			var cloner = new AllocatorCloner<T>(allocator);

			allocatorRegistry.Insert(info.FullName, allocator, cloner);
			allocatorRegistry.Lookup[info.Index] = allocator;

			return allocator;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static AutoAllocator<T> AutoAllocator<T>(this World world) where T : unmanaged
		{
			var info = AllocatorTypeId<T>.Info;
			var allocatorRegistry = world.AllocatorRegistry;

			allocatorRegistry.EnsureLookupAt(info.Index);
			var candidate = allocatorRegistry.Lookup[info.Index];

			if (candidate != null)
			{
				return new AutoAllocator<T>((Allocator<T>)candidate, allocatorRegistry);
			}

			var allocator = new Allocator<T>(DefaultValueUtils.GetDefaultValueFor<T>());
			var cloner = new AllocatorCloner<T>(allocator);

			allocatorRegistry.Insert(info.FullName, allocator, cloner);
			allocatorRegistry.Lookup[info.Index] = allocator;

			return new AutoAllocator<T>(allocator, allocatorRegistry);
		}
		
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ListAllocator<T> ListAllocator<T>(this World world) where T : unmanaged
		{
			return new ListAllocator<T>(world.Allocator<T>(), world.Allocator<int>());
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static AutoListAllocator<T> AutoListAllocator<T>(this World world) where T : unmanaged
		{
			return new AutoListAllocator<T>(world.Allocator<T>(), world.Allocator<int>(), world.AllocatorRegistry);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void AutoFree(this World world, int id, ChunkId chunkId)
		{
			world.AllocatorRegistry.Track(id, chunkId);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void AutoFree(this World world, int id, ListChunkIds listChunkIds)
		{
			world.AllocatorRegistry.Track(id, listChunkIds.Items);
			world.AllocatorRegistry.Track(id, listChunkIds.Count);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static WorkableVar<T> AllocVar<T>(this World world) where T : unmanaged
		{
			var allocator = world.Allocator<T>();
			return new WorkableVar<T>(allocator.Alloc(1), allocator);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static WorkableChunk<T> AllocChunk<T>(this World world, int length = 0) where T : unmanaged
		{
			var allocator = world.Allocator<T>();
			return new WorkableChunk<T>(allocator.Alloc(length), allocator);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static WorkableList<T> AllocList<T>(this World world, int capacity = 0) where T : unmanaged
		{
			return world.ListAllocator<T>().AllocList();
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static WorkableList<T> AllocAutoList<T>(this World world, int id, int capacity = 0) where T : unmanaged
		{
			return world.AutoListAllocator<T>().AllocAutoList(id, capacity);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void Free(this World world, ChunkId chunkId)
		{
			world.AllocatorRegistry.Lookup[chunkId.AllocatorTypeId].Free(chunkId);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void Free(this World world, ListChunkIds listChunkIds)
		{
			world.AllocatorRegistry.Lookup[listChunkIds.Items.AllocatorTypeId].Free(listChunkIds.Items);
			world.AllocatorRegistry.Lookup[listChunkIds.Count.AllocatorTypeId].Free(listChunkIds.Count);
		}
	}
}
