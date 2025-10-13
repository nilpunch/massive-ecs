namespace Massive
{
	public readonly ref struct AllocatorListId
	{
		public readonly ChunkId Items;
		public readonly ChunkId Count;

		/// <summary>
		/// Session-dependent index, used for lookups.
		/// </summary>
		public readonly int ItemsId;

		public AllocatorListId(ChunkId items, ChunkId count, int itemsId)
		{
			Items = items;
			Count = count;
			ItemsId = itemsId;
		}
	}
}
