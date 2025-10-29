using System.Runtime.CompilerServices;

namespace Massive
{
	public readonly unsafe partial struct VarHandleInt
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Free(Allocator allocator)
		{
			allocator.Free(ChunkId);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public VarHandleInt Track(Allocator allocator, int id)
		{
			allocator.Track(id, ChunkId);
			return this;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public ref int Value(Allocator allocator)
		{
			return ref *(int*)(allocator.AlignedPtr + allocator.GetChunk(ChunkId).AlignedOffsetInBytes);
		}
	}
}
