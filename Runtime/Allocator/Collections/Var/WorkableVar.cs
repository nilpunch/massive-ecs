using System.Runtime.CompilerServices;
using Unity.IL2CPP.CompilerServices;

namespace Massive
{
	[Il2CppSetOption(Option.NullChecks, false)]
	[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
	public readonly struct WorkableVar<T> where T : unmanaged
	{
		public readonly ChunkId ChunkId;

		private readonly Allocator<T> _allocator;

		public WorkableVar(ChunkId chunkId, Allocator<T> allocator)
		{
			// Assert.
			allocator.GetChunk(chunkId);

			ChunkId = chunkId;
			_allocator = allocator;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static implicit operator ChunkId(WorkableVar<T> var)
		{
			return var.ChunkId;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static implicit operator VarHandle<T>(WorkableVar<T> chunk)
		{
			return new VarHandle<T>(chunk.ChunkId);
		}

		public ref T Value
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => ref _allocator.Data[_allocator.Chunks[ChunkId.Id].Offset];
		}
	}
}
