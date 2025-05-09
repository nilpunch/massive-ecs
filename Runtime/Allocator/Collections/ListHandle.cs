using System.Runtime.CompilerServices;

namespace Massive
{
	public struct ListHandle<T>
	{
		public ChunkHandle<T> Items;
		public VarHandle<int> Count;

		public ListHandle(ChunkHandle<T> items, VarHandle<int> count)
		{
			Items = items;
			Count = count;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public WorkableList<T> In(ListAllocator<T> allocator)
		{
			return new WorkableList<T>(this, allocator);
		}

		[UnityEngine.Scripting.Preserve]
		private static void ReflectionSupportForAOT()
		{
			_ = new Allocator<T>();
		}
	}
}
