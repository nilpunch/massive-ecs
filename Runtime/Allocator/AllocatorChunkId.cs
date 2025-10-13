namespace Massive
{
	public readonly ref struct AllocatorChunkId
	{
		public readonly ChunkId ChunkId;

		/// <summary>
		/// Session-dependent index, used for lookups.
		/// </summary>
		public readonly int AllocatorId;

		public AllocatorChunkId(ChunkId chunkId, int allocatorId)
		{
			ChunkId = chunkId;
			AllocatorId = allocatorId;
		}
	}
}
