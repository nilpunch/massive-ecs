#if !MASSIVE_DISABLE_ASSERT
#define MASSIVE_ASSERT
#endif

using System.Runtime.CompilerServices;
using Unity.IL2CPP.CompilerServices;

namespace Massive
{
	[Il2CppSetOption(Option.NullChecks, false)]
	[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
	public static class AllocatorCollectionExtensions
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static WorkablePointer<T> AllocVar<T>(this Allocator allocator, T value = default) where T : unmanaged
		{
			var info = Unmanaged<T>.Info;
			var pointer = (Pointer<T>)allocator.Alloc(info.Size, info.Alignment, MemoryInit.Uninitialized);
			pointer.Value(allocator) = value;
			return new WorkablePointer<T>(pointer, allocator);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static WorkableArray<T> AllocArray<T>(this Allocator allocator, int length, MemoryInit memoryInit = MemoryInit.Clear) where T : unmanaged
		{
			var info = Unmanaged<T>.Info;

			var pointer = new ArrayPointer<T>((Pointer<ArrayModel<T>>)allocator.Alloc(ArrayModel<T>.Size, ArrayModel<T>.Alignment, MemoryInit.Uninitialized));

			ref var model = ref pointer.Model.Value(allocator);
			model.Items = (Pointer<T>)allocator.Alloc(length * info.Size, info.Alignment, memoryInit);
			model.Length = length;

			return new WorkableArray<T>(pointer, allocator);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static WorkableList<T> AllocList<T>(this Allocator allocator, int capacity = 0) where T : unmanaged
		{
			var info = Unmanaged<T>.Info;

			var pointer = new ListPointer<T>((Pointer<ListModel<T>>)allocator.Alloc(ListModel<T>.Size, ListModel<T>.Alignment, MemoryInit.Uninitialized));

			ref var model = ref pointer.Model.Value(allocator);
			model.Items = (Pointer<T>)allocator.Alloc(capacity * info.Size, info.Alignment, MemoryInit.Uninitialized);
			model.Capacity = capacity;
			model.Count = 0;

			return new WorkableList<T>(pointer, allocator);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ArrayModel<T> AllocArrayModel<T>(this Allocator allocator, int length, MemoryInit memoryInit = MemoryInit.Clear) where T : unmanaged
		{
			var info = Unmanaged<T>.Info;

			ArrayModel<T> model = default;
			model.Items = (Pointer<T>)allocator.Alloc(length * info.Size, info.Alignment, memoryInit);
			model.Length = length;

			return model;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ListModel<T> AllocListModel<T>(this Allocator allocator, int capacity = 0) where T : unmanaged
		{
			var info = Unmanaged<T>.Info;

			ListModel<T> model = default;
			model.Items = (Pointer<T>)allocator.Alloc(capacity * info.Size, info.Alignment, MemoryInit.Uninitialized);
			model.Capacity = capacity;
			model.Count = 0;

			return model;
		}
	}
}
