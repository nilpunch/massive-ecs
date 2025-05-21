using System.Runtime.CompilerServices;
using Unity.IL2CPP.CompilerServices;

namespace Massive
{
	[Il2CppSetOption(Option.NullChecks, false)]
	[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
	public readonly struct ListHandle<T> where T : unmanaged
	{
		public readonly ChunkId Items;
		public readonly ChunkId Count;

		public ListHandle(ChunkId items, ChunkId count)
		{
			Items = items;
			Count = count;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public WorkableList<T> In(World world)
		{
			return new WorkableList<T>(Items, Count,
				(Allocator<T>)world.Allocators.Lookup[AllocatorId<T>.Index],
				world.Allocators.IntAllocator);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public WorkableList<T> In(ListAllocator<T> allocator)
		{
			return new WorkableList<T>(Items, Count, allocator.Items, allocator.Count);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static implicit operator ListChunkIds(ListHandle<T> handle)
		{
			return new ListChunkIds(handle.Items, handle.Count, AllocatorId<T>.Index);
		}

		[UnityEngine.Scripting.Preserve]
		private static void ReflectionSupportForAOT()
		{
			_ = new Allocator<T>();
			_ = new Allocator<int>();
		}
	}
}
