using System.Runtime.CompilerServices;
using Unity.IL2CPP.CompilerServices;

namespace Massive
{
	[Il2CppEagerStaticClassConstruction]
	[Il2CppSetOption(Option.NullChecks, false)]
	[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
	public static class WorldAllocatorExtensions
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static WorkableVar<T> AllocVar<T>(this World world, T value = default) where T : unmanaged
		{
			return world.Allocator.AllocVar(value);
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static WorkableArray<T> AllocArray<T>(this World world, int minimumLength, MemoryInit memoryInit = MemoryInit.Clear) where T : unmanaged
		{
			return world.Allocator.AllocArray<T>(minimumLength, memoryInit);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static WorkableList<T> AllocList<T>(this World world, int capacity = 0) where T : unmanaged
		{
			return world.Allocator.AllocList<T>(capacity);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void Free(this World world, ChunkId chunkId)
		{
			world.Allocator.Free(chunkId);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void Free(this World world, ListId listId)
		{
			world.Allocator.Free(listId.Items);
			world.Allocator.Free(listId.Count);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void Track(this World world, int id, ChunkId chunkId)
		{
			world.Allocator.Track(id, chunkId);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void Track(this World world, int id, ListId listId)
		{
			world.Allocator.Track(id, listId.Items);
			world.Allocator.Track(id, listId.Count);
		}
	}
}
