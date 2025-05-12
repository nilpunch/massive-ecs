using System.Runtime.CompilerServices;
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

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public WorkableList<T> AllocList(int capacity = 0)
		{
			var count = Count.Alloc(1, MemoryInit.Uninitialized);
			Count.Data[Count.Chunks[count.Id].Offset] = 0;

			return new WorkableList<T>(
				Items.Alloc(capacity, MemoryInit.Uninitialized),
				count,
				this);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Free(ListHandle<T> handle)
		{
			Items.Free(handle.Items);
			Count.Free(handle.Count);
		}
	}
}
