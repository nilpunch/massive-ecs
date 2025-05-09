using System.Runtime.CompilerServices;

namespace Massive
{
	public struct VarHandle<T>
	{
		public ChunkId ChunkId;

		public VarHandle(ChunkId chunkId)
		{
			ChunkId = chunkId;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public WorkableVar<T> WorkWith(Allocator<T> allocator)
		{
			return new WorkableVar<T>(ChunkId, allocator);
		}

		[UnityEngine.Scripting.Preserve]
		private static void ReflectionSupportForAOT()
		{
			_ = new Allocator<T>();
		}
	}
}
