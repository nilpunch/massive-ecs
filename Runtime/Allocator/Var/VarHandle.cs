using System.Runtime.CompilerServices;
using Unity.IL2CPP.CompilerServices;

namespace Massive
{
	[Il2CppSetOption(Option.NullChecks, false)]
	[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
	public readonly partial struct VarHandle<T> where T : unmanaged
	{
		public readonly Pointer<T> Pointer;

		public VarHandle(Pointer<T> pointer)
		{
			Pointer = pointer;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public WorkableVar<T> In(Allocator allocator)
		{
			return new WorkableVar<T>(this, allocator);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static implicit operator Pointer(VarHandle<T> handle)
		{
			return handle.Pointer;
		}
	}
}
