using System;
using System.Runtime.CompilerServices;

namespace Massive
{
	public class ChunkUnknownException : Exception
	{
		private ChunkUnknownException(string message) : base(message)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public static void Throw(ChunkId chunkId)
		{
			throw new ChunkUnknownException($"{MassiveAssert.Library} Chunk with id:{chunkId.Id} v:{chunkId.Version} not found.");
		}
	}
}
