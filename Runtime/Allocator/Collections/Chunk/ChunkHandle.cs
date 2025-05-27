using System.Runtime.CompilerServices;
using Unity.IL2CPP.CompilerServices;

namespace Massive
{
	[Il2CppSetOption(Option.NullChecks, false)]
	[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
	public readonly struct ChunkHandle<T> where T : unmanaged
	{
		public readonly ChunkId ChunkId;

		public ChunkHandle(ChunkId chunkId)
		{
			ChunkId = chunkId;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public WorkableChunk<T> In(World world)
		{
			return new WorkableChunk<T>(ChunkId, (Allocator<T>)world.Allocators.Lookup[AllocatorId<T>.Index]);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public WorkableChunk<T> In(Allocator<T> allocator)
		{
			return new WorkableChunk<T>(ChunkId, allocator);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public WorkableChunk<T> In(AutoAllocator<T> allocator)
		{
			return new WorkableChunk<T>(ChunkId, allocator.Allocator);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static implicit operator AllocatorChunkId(ChunkHandle<T> handle)
		{
			return new AllocatorChunkId(handle.ChunkId, AllocatorId<T>.Index);
		}

		[UnityEngine.Scripting.Preserve]
		private static void ReflectionSupportForAOT()
		{
			_ = new Allocator<T>();
		}
	}
}
