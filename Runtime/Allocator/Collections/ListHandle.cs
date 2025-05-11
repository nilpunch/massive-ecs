using System.Runtime.CompilerServices;
using Unity.IL2CPP.CompilerServices;

namespace Massive
{
	[Il2CppSetOption(Option.NullChecks, false)]
	[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
	public struct ListHandle<T> where T : unmanaged
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
