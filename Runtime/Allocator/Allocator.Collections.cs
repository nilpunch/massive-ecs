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
		public WorkableVarInt AllocInt(int value = default)
		{
			var varHandle = new VarHandleInt(Alloc(sizeof(int), sizeof(int), MemoryInit.Uninitialized));
			varHandle.Value(this) = value;
			return new WorkableVarInt(varHandle, this);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public WorkableVar<T> AllocVar<T>(T value = default) where T : unmanaged
		{
			var info = Unmanaged<T>.Info;
			var varHandle = new VarHandle<T>(Alloc(info.Size, info.Alignment, MemoryInit.Uninitialized));
			varHandle.Value(this) = value;
			return new WorkableVar<T>(varHandle, this);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public WorkableArray<T> AllocArray<T>(int length, MemoryInit memoryInit = MemoryInit.Clear) where T : unmanaged
		{
			var info = Unmanaged<T>.Info;

			var pointer = new ArrayPointer<T>(Alloc(ArrayModel.Size, ArrayModel.Alignment, MemoryInit.Uninitialized));

			ref var model = ref pointer.GetModel(this);
			model.Items = Alloc(length * info.Size, info.Alignment, memoryInit);
			model.Length = length;

			return new WorkableArray<T>(pointer, this);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public WorkableList<T> AllocList<T>(int capacity = 0) where T : unmanaged
		{
			var info = Unmanaged<T>.Info;

			var pointer = new ListPointer<T>(Alloc(ListModel.Size, ListModel.Alignment, MemoryInit.Uninitialized));

			ref var model = ref pointer.GetModel(this);
			model.Items = Alloc(capacity * info.Size, info.Alignment, MemoryInit.Uninitialized);
			model.Capacity = capacity;
			model.Count = 0;

			return new WorkableList<T>(pointer, this);
		}
	}
}
