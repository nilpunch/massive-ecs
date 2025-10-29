namespace Massive
{
	public readonly ref struct ListId
	{
		public readonly ChunkId Items;
		public readonly ChunkId Count;

		public ListId(ChunkId items, ChunkId count)
		{
			Items = items;
			Count = count;
		}
	}
}
