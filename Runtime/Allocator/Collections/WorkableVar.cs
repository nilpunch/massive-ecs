using System.Runtime.CompilerServices;
using Unity.IL2CPP.CompilerServices;

namespace Massive
{
	[Il2CppSetOption(Option.NullChecks, false)]
	[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
	public readonly unsafe ref struct WorkableVar<T>
	{
		private readonly ChunkId _chunkId;
		private readonly Chunk* _chunk;
		private readonly Allocator<T> _allocator;

		public WorkableVar(ChunkId chunkId, Allocator<T> allocator)
		{
			_chunkId = chunkId;

			fixed (Chunk* chunkPtr = &allocator.GetChunk(_chunkId))
			{
				_chunk = chunkPtr;
			}

			_allocator = allocator;
		}

		public VarHandle<T> AsHandle => new VarHandle<T>(_chunkId);

		public ref T Value
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => ref _allocator.Data[_chunk->Offset];
		}
	}
}
