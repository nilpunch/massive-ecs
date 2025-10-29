using System.Runtime.CompilerServices;

namespace Massive
{
	public readonly unsafe partial struct VarHandle<T>
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Free(Allocator allocator)
		{
			allocator.Free(ChunkId);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public VarHandle<T> Track(Allocator allocator, int id)
		{
			allocator.Track(id, ChunkId);
			return this;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public ref T Value(Allocator allocator)
		{
			return ref *(T*)(allocator.AlignedPtr + allocator.GetChunk(ChunkId).AlignedOffsetInBytes);
		}
	}
}
