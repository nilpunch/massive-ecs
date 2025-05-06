using System;
using System.Runtime.CompilerServices;
using Unity.IL2CPP.CompilerServices;

namespace Massive
{
	[Il2CppSetOption(Option.NullChecks, false)]
	[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
	public static class WorldAllocatorExtensions
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Allocator<T> Allocator<T>(this World world)
		{
			throw new NotImplementedException();
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ListAllocator<T> ListAllocator<T>(this World world)
		{
			return new ListAllocator<T>(world);
		}
	}
}
