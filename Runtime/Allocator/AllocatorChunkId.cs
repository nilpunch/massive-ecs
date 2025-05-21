namespace Massive
{
	public readonly ref struct AllocatorChunkId
	{
		public readonly ChunkId ChunkId;

		/// <summary>
		/// Non-deterministic, used for lookups.<br/>
		/// Don't store it in simulation.
		/// </summary>
		public readonly int AllocatorId;

		public AllocatorChunkId(ChunkId chunkId, int allocatorId)
		{
			ChunkId = chunkId;
			AllocatorId = allocatorId;
		}
	}
}
