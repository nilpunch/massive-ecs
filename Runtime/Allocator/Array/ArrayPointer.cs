using System.Runtime.CompilerServices;
using Unity.IL2CPP.CompilerServices;

namespace Massive
{
	[Il2CppSetOption(Option.NullChecks, false)]
	[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
	public readonly partial struct ArrayPointer<T> where T : unmanaged
	{
		public readonly Pointer<ArrayModel<T>> ModelPointer;

		public ArrayPointer(Pointer<ArrayModel<T>> modelPointer)
		{
			ModelPointer = modelPointer;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public WorkableArray<T> In(Allocator allocator)
		{
			return new WorkableArray<T>(this, allocator);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static implicit operator Pointer(ArrayPointer<T> pointer)
		{
			return pointer.ModelPointer.AsPointer;
		}
	}
}
