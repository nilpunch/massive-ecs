using System.Runtime.CompilerServices;
using Unity.IL2CPP.CompilerServices;

namespace Massive
{
	[Il2CppSetOption(Option.NullChecks, false)]
	[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
	public readonly struct AutoListAllocator<T> where T : unmanaged
	{
		public readonly Allocator<T> Items;
		public readonly Allocator<int> Count;
		public readonly Allocators Registry;

		public AutoListAllocator(Allocator<T> items, Allocator<int> count, Allocators registry)
		{
			Items = items;
			Count = count;
			Registry = registry;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public WorkableList<T> AllocList(int capacity = 0)
		{
			var items = Items.Alloc(capacity, MemoryInit.Uninitialized);

			var count = Count.Alloc(1, MemoryInit.Uninitialized);
			Count.Data[Count.Chunks[count.Id].Offset] = 0;

			return new WorkableList<T>(items, count, Items, Count);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public WorkableList<T> AllocAutoList(int id, int capacity = 0)
		{
			var items = Items.Alloc(capacity, MemoryInit.Uninitialized);

			var count = Count.Alloc(1, MemoryInit.Uninitialized);
			Count.Data[Count.Chunks[count.Id].Offset] = 0;

			Registry.Track(id, items);
			Registry.Track(id, count);

			return new WorkableList<T>(items, count, Items, Count);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Free(ListChunkIds listChunkIds)
		{
			Items.Free(listChunkIds.Items);
			Count.Free(listChunkIds.Count);
		}
	}
}
