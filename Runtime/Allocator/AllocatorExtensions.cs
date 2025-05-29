using System.Runtime.CompilerServices;
using Unity.IL2CPP.CompilerServices;

namespace Massive
{
	[Il2CppSetOption(Option.NullChecks, false)]
	[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
	public static class AllocatorExtensions
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static WorkableVar<T> AllocVar<T>(this Allocator<T> allocator, T value = default) where T : unmanaged
		{
			var chunkId = allocator.Alloc(1);
			allocator.Data[allocator.Chunks[chunkId.Id].Offset] = value;
			return new WorkableVar<T>(chunkId, allocator);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static WorkableArray<T> AllocArray<T>(this Allocator<T> allocator, int minimumLength, MemoryInit memoryInit = MemoryInit.Clear) where T : unmanaged
		{
			return new WorkableArray<T>(allocator.Alloc(minimumLength, memoryInit), allocator);
		}
	}
}
