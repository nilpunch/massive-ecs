using System.Runtime.CompilerServices;
using Unity.IL2CPP.CompilerServices;

namespace Massive
{
	[Il2CppSetOption(Option.NullChecks, false)]
	[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
	public readonly partial struct ListPointer<T> where T : unmanaged
	{
		public readonly Pointer<ListModel<T>> ModelPointer;

		public ListPointer(Pointer<ListModel<T>> modelPointer)
		{
			ModelPointer = modelPointer;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public WorkableList<T> In(Allocator allocator)
		{
			return new WorkableList<T>(this, allocator);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static implicit operator Pointer(ListPointer<T> pointer)
		{
			return pointer.ModelPointer.AsPointer;
		}
	}
}
