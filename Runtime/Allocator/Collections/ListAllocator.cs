using Unity.IL2CPP.CompilerServices;

namespace Massive
{
	/// <summary>
	/// Wrapper for all allocator required for list to work.
	/// </summary>
	[Il2CppSetOption(Option.NullChecks, false)]
	[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
	public readonly struct ListAllocator<T> where T : unmanaged
	{
		public readonly Allocator<T> Items;
		public readonly Allocator<int> Count;

		public ListAllocator(Allocator<T> items, Allocator<int> count)
		{
			Items = items;
			Count = count;
		}
	}
}
