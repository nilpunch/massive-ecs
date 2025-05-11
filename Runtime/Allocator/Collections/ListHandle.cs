using System.Runtime.CompilerServices;
using Unity.IL2CPP.CompilerServices;

namespace Massive
{
	[Il2CppSetOption(Option.NullChecks, false)]
	[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
	public struct ListHandle<T> where T : unmanaged
	{
		public ChunkId Items;
		public ChunkId Count;

		public ListHandle(ChunkId items, ChunkId count)
		{
			Items = items;
			Count = count;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public WorkableList<T> In(ListAllocator<T> allocator)
		{
			return new WorkableList<T>(Items, Count, allocator);
		}

		[UnityEngine.Scripting.Preserve]
		private static void ReflectionSupportForAOT()
		{
			_ = new Allocator<T>();
		}
	}
}
