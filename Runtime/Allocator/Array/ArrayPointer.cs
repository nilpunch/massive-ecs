using System.Runtime.CompilerServices;
using Unity.IL2CPP.CompilerServices;

namespace Massive
{
	[Il2CppSetOption(Option.NullChecks, false)]
	[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
	public readonly partial struct ArrayPointer<T> where T : unmanaged
	{
		public readonly Pointer<ArrayModel<T>> Model;

		public ArrayPointer(Pointer<ArrayModel<T>> model)
		{
			Model = model;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public WorkableArray<T> In(Allocator allocator)
		{
			return new WorkableArray<T>(this, allocator);
		}
	}
}
