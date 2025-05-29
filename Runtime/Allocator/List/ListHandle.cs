using System.Runtime.CompilerServices;
using Unity.IL2CPP.CompilerServices;

namespace Massive
{
	[Il2CppSetOption(Option.NullChecks, false)]
	[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
	public readonly struct ListHandle<T> where T : unmanaged
	{
		private readonly ChunkId _items;
		private readonly ChunkId _count;

		public ListHandle(ChunkId items, ChunkId count)
		{
			_items = items;
			_count = count;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public WorkableList<T> In(World world)
		{
			return new WorkableList<T>(_items, _count,
				(Allocator<T>)world.Allocators.Lookup[AllocatorId<T>.Index],
				world.Allocators.IntAllocator);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public WorkableList<T> In(AutoAllocator<T> allocator)
		{
			return new WorkableList<T>(_items, _count, allocator.Allocator, allocator.Allocators.IntAllocator);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static implicit operator AllocatorListId(ListHandle<T> handle)
		{
			return new AllocatorListId(handle._items, handle._count, AllocatorId<T>.Index);
		}

		[UnityEngine.Scripting.Preserve]
		private static void ReflectionSupportForAOT()
		{
			_ = new Allocator<T>();
			_ = new Allocator<int>();
		}
	}
}
