namespace Massive
{
	/// <summary>
	/// Wrapper for all allocator required for list to work.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public readonly struct ListAllocator<T>
	{
		public readonly Allocator<T> Items;
		public readonly Allocator<int> Count;

		public ListAllocator(World world)
		{
			Items = world.Allocator<T>();
			Count = world.Allocator<int>();
		}
	}
}
