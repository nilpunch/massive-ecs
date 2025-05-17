using System.Runtime.CompilerServices;
using Unity.IL2CPP.CompilerServices;

namespace Massive
{
	[Il2CppSetOption(Option.NullChecks, false)]
	[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
	public readonly struct VarHandle<T> where T : unmanaged
	{
		public readonly ChunkId ChunkId;

		public VarHandle(ChunkId chunkId)
		{
			ChunkId = chunkId;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public WorkableVar<T> In(World world)
		{
			return new WorkableVar<T>(ChunkId, (Allocator<T>)world.Allocators.Lookup[ChunkId.AllocatorTypeId]);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public WorkableVar<T> In(Allocator<T> allocator)
		{
			return new WorkableVar<T>(ChunkId, allocator);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public WorkableVar<T> In(AutoAllocator<T> allocator)
		{
			return new WorkableVar<T>(ChunkId, allocator.Allocator);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static implicit operator ChunkId(VarHandle<T> handle)
		{
			return handle.ChunkId;
		}

		[UnityEngine.Scripting.Preserve]
		private static void ReflectionSupportForAOT()
		{
			_ = new Allocator<T>();
		}
	}
}
