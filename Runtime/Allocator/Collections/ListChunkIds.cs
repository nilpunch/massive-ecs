namespace Massive
{
	public struct ListChunkIds
	{
		public ChunkId Items;
		public ChunkId Count;

		public ListChunkIds(ChunkId items, ChunkId count)
		{
			Items = items;
			Count = count;
		}
	}
}
