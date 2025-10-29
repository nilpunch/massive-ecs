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
			var arrayHandle = new ArrayHandle<T>(Alloc(length * info.Size, info.Alignment, memoryInit));
			return new WorkableArray<T>(arrayHandle, this);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public WorkableList<T> AllocList<T>(int capacity = 0) where T : unmanaged
		{
			var info = Unmanaged<T>.Info;
			var items = new ArrayHandle<T>(Alloc(capacity * info.Size, info.Alignment, MemoryInit.Uninitialized));
			var count = new VarHandleInt(Alloc(sizeof(int), sizeof(int), MemoryInit.Uninitialized));
			count.Value(this) = 0;

			return new WorkableList<T>(new ListHandle<T>(items, count), this);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public ListHandle<T> AllocListHandle<T>(int capacity = 0) where T : unmanaged
		{
			var info = Unmanaged<T>.Info;
			var items = new ArrayHandle<T>(Alloc(capacity * info.Size, info.Alignment, MemoryInit.Uninitialized));
			var count = new VarHandleInt(Alloc(sizeof(int), sizeof(int), MemoryInit.Uninitialized));
			count.Value(this) = 0;

			return new ListHandle<T>(items, count);
		}
	}
}
