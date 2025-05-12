using System;
using System.Runtime.CompilerServices;

namespace Massive
{
	public class ChunkFromOtherAllocatorException : Exception
	{
		private ChunkFromOtherAllocatorException(string message) : base(message)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public static void Throw(ChunkId chunkId)
		{
			throw new ChunkFromOtherAllocatorException(
				$"{MassiveAssert.Library} Chunk with id:{chunkId.Id} v:{chunkId.Version} is from another allocator of type " +
				$"{AllocatorTypeId.GetTypeByIndex(chunkId.Allocator).GetGenericName()}.");
		}
	}
}
