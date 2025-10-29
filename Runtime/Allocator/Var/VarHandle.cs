using System.Runtime.CompilerServices;
using Unity.IL2CPP.CompilerServices;

namespace Massive
{
	[Il2CppSetOption(Option.NullChecks, false)]
	[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
	public readonly partial struct VarHandle<T> where T : unmanaged
	{
		public readonly ChunkId ChunkId;

		public VarHandle(ChunkId chunkId)
		{
			ChunkId = chunkId;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public WorkableVar<T> In(Allocator allocator)
		{
			return new WorkableVar<T>(this, allocator);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static implicit operator ChunkId(VarHandle<T> handle)
		{
			return handle.ChunkId;
		}
	}
}
