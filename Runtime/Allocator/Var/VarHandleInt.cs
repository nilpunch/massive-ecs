using System.Runtime.CompilerServices;
using Unity.IL2CPP.CompilerServices;

namespace Massive
{
	[Il2CppSetOption(Option.NullChecks, false)]
	[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
	public readonly partial struct VarHandleInt
	{
		public readonly Pointer Pointer;

		public VarHandleInt(Pointer pointer)
		{
			Pointer = pointer;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public WorkableVarInt In(Allocator allocator)
		{
			return new WorkableVarInt(this, allocator);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static implicit operator Pointer(VarHandleInt handle)
		{
			return handle.Pointer;
		}
	}
}
