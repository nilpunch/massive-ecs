namespace Massive
{
	public readonly ref struct ListChunkIds
	{
		public readonly ChunkId Items;
		public readonly ChunkId Count;

		public readonly int ItemsId;

		public ListChunkIds(ChunkId items, ChunkId count, int itemsId)
		{
			Items = items;
			Count = count;
			ItemsId = itemsId;
		}
	}
}
