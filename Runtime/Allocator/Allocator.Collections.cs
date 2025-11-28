#if !MASSIVE_DISABLE_ASSERT
#define MASSIVE_ASSERT
#endif

using System.Runtime.CompilerServices;
using Unity.IL2CPP.CompilerServices;

namespace Massive
{
	[Il2CppSetOption(Option.NullChecks, false)]
	[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
	public partial class Allocator
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public WorkablePointer<T> AllocVar<T>(T value = default) where T : unmanaged
		{
			var info = Unmanaged<T>.Info;
			var pointer = (Pointer<T>)Alloc(info.Size, info.Alignment, MemoryInit.Uninitialized);
			pointer.Value(this) = value;
			return new WorkablePointer<T>(pointer, this);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public WorkableArray<T> AllocArray<T>(int length, MemoryInit memoryInit = MemoryInit.Clear) where T : unmanaged
		{
			var info = Unmanaged<T>.Info;

			var pointer = new ArrayPointer<T>((Pointer<ArrayModel<T>>)Alloc(ArrayModel<T>.Size, ArrayModel<T>.Alignment, MemoryInit.Uninitialized));

			ref var model = ref pointer.Model.Value(this);
			model.Items = (Pointer<T>)Alloc(length * info.Size, info.Alignment, memoryInit);
			model.Length = length;

			return new WorkableArray<T>(pointer, this);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public WorkableList<T> AllocList<T>(int capacity = 0) where T : unmanaged
		{
			var info = Unmanaged<T>.Info;

			var pointer = new ListPointer<T>((Pointer<ListModel<T>>)Alloc(ListModel<T>.Size, ListModel<T>.Alignment, MemoryInit.Uninitialized));

			ref var model = ref pointer.Model.Value(this);
			model.Items = (Pointer<T>)Alloc(capacity * info.Size, info.Alignment, MemoryInit.Uninitialized);
			model.Capacity = capacity;
			model.Count = 0;

			return new WorkableList<T>(pointer, this);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public ArrayModel<T> AllocArrayModel<T>(int length, MemoryInit memoryInit = MemoryInit.Clear) where T : unmanaged
		{
			var info = Unmanaged<T>.Info;

			ArrayModel<T> model = default;
			model.Items = (Pointer<T>)Alloc(length * info.Size, info.Alignment, memoryInit);
			model.Length = length;

			return model;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public ListModel<T> AllocListModel<T>(int capacity = 0) where T : unmanaged
		{
			var info = Unmanaged<T>.Info;

			ListModel<T> model = default;
			model.Items = (Pointer<T>)Alloc(capacity * info.Size, info.Alignment, MemoryInit.Uninitialized);
			model.Capacity = capacity;
			model.Count = 0;

			return model;
		}
	}
}
