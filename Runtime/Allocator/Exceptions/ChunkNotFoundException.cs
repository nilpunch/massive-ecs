using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Massive
{
	public class ChunkNotFoundException : MassiveException
	{
		private ChunkNotFoundException(string message) : base(message)
		{
		}

		[Conditional(Condition)]
		[MethodImpl(MethodImplOptions.NoInlining)]
		public static void ThrowIfNotInCountRange(Allocator allocator, ChunkId chunkId)
		{
			if (chunkId.Id < 0 || chunkId.Id >= allocator.ChunkCount)
			{
				throw new ChunkNotFoundException($"Chunk with id:{chunkId.Id} v:{chunkId.Version} not found.");
			}
		}

		[Conditional(Condition)]
		[MethodImpl(MethodImplOptions.NoInlining)]
		public static void ThrowIfDeallocated(in Chunk chunk, ChunkId chunkId)
		{
			if (chunk.Length < 0 || chunk.Version != chunkId.Version)
			{
				throw new ChunkNotFoundException($"Chunk with id:{chunkId.Id} v:{chunkId.Version} not found.");
			}
		}

		[Conditional(Condition)]
		[MethodImpl(MethodImplOptions.NoInlining)]
		public static void ThrowIfFromOtherAllocator(Allocator allocator, ChunkId chunkId)
		{
			if (allocator.AllocatorTypeId != chunkId.AllocatorTypeId)
			{
				throw new ChunkNotFoundException(
					$"Chunk with id:{chunkId.Id} v:{chunkId.Version} is from allocator of different type " +
					$"{AllocatorTypeId.GetTypeByIndex(chunkId.AllocatorTypeId).GetGenericName()}.");
			}
		}
	}
}
