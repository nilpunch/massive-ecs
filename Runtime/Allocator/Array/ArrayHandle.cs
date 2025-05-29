using System.Runtime.CompilerServices;
using Unity.IL2CPP.CompilerServices;

namespace Massive
{
	[Il2CppSetOption(Option.NullChecks, false)]
	[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
	public readonly struct ArrayHandle<T> where T : unmanaged
	{
		public readonly ChunkId ChunkId;

		public ArrayHandle(ChunkId chunkId)
		{
			ChunkId = chunkId;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public WorkableArray<T> In(World world)
		{
			return new WorkableArray<T>(ChunkId, (Allocator<T>)world.Allocators.Lookup[AllocatorId<T>.Index]);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public WorkableArray<T> In(Allocator<T> allocator)
		{
			return new WorkableArray<T>(ChunkId, allocator);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public WorkableArray<T> In(AutoAllocator<T> allocator)
		{
			return new WorkableArray<T>(ChunkId, allocator.Allocator);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static implicit operator AllocatorChunkId(ArrayHandle<T> handle)
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
