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
			var info = TypeId<T>.Info;
			var allocatorRegistry = world.AllocatorRegistry;

			allocatorRegistry.EnsureLookupAt(info.Index);
			var candidate = allocatorRegistry.Lookup[info.Index];

			if (candidate != null)
			{
				return (Allocator<T>)candidate;
			}

			var allocator = new Allocator<T>();
			var cloner = new AllocatorCloner<T>(allocator);

			allocatorRegistry.Insert(info.FullName, allocator, cloner);
			allocatorRegistry.Lookup[info.Index] = allocator;

			return allocator;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ListAllocator<T> ListAllocator<T>(this World world)
		{
			return new ListAllocator<T>(world.Allocator<T>(), world.Allocator<int>());
		}
		
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static WorkableVar<T> AllocVar<T>(this World world)
		{
			var allocator = world.Allocator<T>();
			return new WorkableVar<T>(allocator.Alloc(1), allocator);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static WorkableChunk<T> AllocChunk<T>(this World world, int length = 0)
		{
			var allocator = world.Allocator<T>();
			return new WorkableChunk<T>(allocator.Alloc(length), allocator);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static WorkableList<T> AllocList<T>(this World world, int capacity = 0)
		{
			var allocator = world.ListAllocator<T>();
			return new WorkableList<T>(new ListHandle<T>(
					new ChunkHandle<T>(allocator.Items.Alloc(capacity)),
					new VarHandle<int>(allocator.Count.Alloc(1))),
				allocator);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void Free<T>(this World world, VarHandle<T> handle)
		{
			var allocator = world.Allocator<T>();
			allocator.Free(handle.ChunkId);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void Free<T>(this World world, ChunkHandle<T> handle)
		{
			var allocator = world.Allocator<T>();
			allocator.Free(handle.ChunkId);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void Free<T>(this World world, ListHandle<T> handle)
		{
			var allocator = world.ListAllocator<T>();
			allocator.Items.Free(handle.Items.ChunkId);
			allocator.Count.Free(handle.Count.ChunkId);
		}
	}
}
