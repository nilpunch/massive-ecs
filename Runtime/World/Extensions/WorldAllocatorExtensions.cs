using System.Runtime.CompilerServices;
using Unity.IL2CPP.CompilerServices;

namespace Massive
{
	[Il2CppEagerStaticClassConstruction]
	[Il2CppSetOption(Option.NullChecks, false)]
	[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
	public static class WorldAllocatorExtensions
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static WorkablePointer<T> AllocVar<T>(this World world, T value = default) where T : unmanaged
		{
			return world.Allocator.AllocVar(value);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static WorkableArray<T> AllocArray<T>(this World world, int length, MemoryInit memoryInit = MemoryInit.Clear) where T : unmanaged
		{
			return world.Allocator.AllocArray<T>(length, memoryInit);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static WorkableList<T> AllocList<T>(this World world, int capacity = 0) where T : unmanaged
		{
			return world.Allocator.AllocList<T>(capacity);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ArrayModel<T> AllocArrayModel<T>(this World world, int length, MemoryInit memoryInit = MemoryInit.Clear) where T : unmanaged
		{
			return world.Allocator.AllocArrayModel<T>(length, memoryInit);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ListModel<T> AllocListModel<T>(this World world, int capacity = 0) where T : unmanaged
		{
			return world.Allocator.AllocListModel<T>(capacity);
		}
	}
}
