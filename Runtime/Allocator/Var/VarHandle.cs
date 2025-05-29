using System.Runtime.CompilerServices;
using Unity.IL2CPP.CompilerServices;

namespace Massive
{
	[Il2CppSetOption(Option.NullChecks, false)]
	[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
	public readonly struct VarHandle<T> where T : unmanaged
	{
		private readonly ChunkId _chunkId;

		public VarHandle(ChunkId chunkId)
		{
			_chunkId = chunkId;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public WorkableVar<T> In(World world)
		{
			return new WorkableVar<T>(_chunkId, (Allocator<T>)world.Allocators.Lookup[AllocatorId<T>.Index]);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public WorkableVar<T> In(Allocator<T> allocator)
		{
			return new WorkableVar<T>(_chunkId, allocator);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public WorkableVar<T> In(AutoAllocator<T> allocator)
		{
			return new WorkableVar<T>(_chunkId, allocator.Allocator);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static implicit operator AllocatorChunkId(VarHandle<T> handle)
		{
			return new AllocatorChunkId(handle._chunkId, AllocatorId<T>.Index);
		}

		[UnityEngine.Scripting.Preserve]
		private static void ReflectionSupportForAOT()
		{
			_ = new Allocator<T>();
		}
	}
}
