using System.Runtime.CompilerServices;

namespace Massive
{
	public struct ChunkHandle<T>
	{
		public ChunkId ChunkId;

		public ChunkHandle(ChunkId chunkId)
		{
			ChunkId = chunkId;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public WorkableChunk<T> WorkWith(World world)
		{
			return new WorkableChunk<T>(ChunkId, world.Allocator<T>());
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public WorkableChunk<T> WorkWith(Allocator<T> allocator)
		{
			return new WorkableChunk<T>(ChunkId, allocator);
		}

		[UnityEngine.Scripting.Preserve]
		private static void ReflectionSupportForAOT()
		{
			_ = new Allocator<T>();
		}
	}
}
