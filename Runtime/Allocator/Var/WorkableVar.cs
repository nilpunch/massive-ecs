using System.Runtime.CompilerServices;
using Unity.IL2CPP.CompilerServices;

namespace Massive
{
	[Il2CppSetOption(Option.NullChecks, false)]
	[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
	public readonly ref struct WorkableVar<T> where T : unmanaged
	{
		public readonly ChunkId ChunkId;
		public readonly Allocator<T> Allocator;

		public WorkableVar(ChunkId chunkId, Allocator<T> allocator)
		{
			// Assert.
			allocator.GetChunk(chunkId);

			ChunkId = chunkId;
			Allocator = allocator;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static implicit operator AllocatorChunkId(WorkableVar<T> var)
		{
			return new AllocatorChunkId(var.ChunkId, var.Allocator.AllocatorId);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static implicit operator VarHandle<T>(WorkableVar<T> chunk)
		{
			return new VarHandle<T>(chunk.ChunkId);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Free()
		{
			Allocator.Free(ChunkId);
		}

		public ref T Value
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => ref Allocator.Data[Allocator.Chunks[ChunkId.Id].Offset];
		}
	}
}
