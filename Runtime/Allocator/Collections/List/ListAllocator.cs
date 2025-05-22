using System.Runtime.CompilerServices;
using Unity.IL2CPP.CompilerServices;

namespace Massive
{
	[Il2CppSetOption(Option.NullChecks, false)]
	[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
	public readonly struct ListAllocator<T> where T : unmanaged
	{
		public readonly Allocator<T> Items;
		public readonly Allocator<int> Count;
		public readonly Allocators Allocators;
		public readonly int ItemsTypeId;
		public readonly int CountTypeId;

		public ListAllocator(Allocators allocators)
		{
			Allocators = allocators;
			Items = (Allocator<T>)allocators.Get<T>();
			ItemsTypeId = AllocatorId<T>.Index;
			Count = allocators.IntAllocator;
			CountTypeId = allocators.IntId;
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

			Allocators.TrackAllocation(id, items, ItemsTypeId);
			Allocators.TrackAllocation(id, count, CountTypeId);

			return new WorkableList<T>(items, count, Items, Count);
		}
	}
}
