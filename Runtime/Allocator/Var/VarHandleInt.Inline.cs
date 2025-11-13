using System.Runtime.CompilerServices;

namespace Massive
{
	public readonly unsafe partial struct VarHandleInt
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Free(Allocator allocator)
		{
			allocator.Free(Pointer);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public ref int Value(Allocator allocator)
		{
			return ref *(int*)(allocator.GetPage(Pointer).AlignedPtr + Pointer.Offset);
		}
	}
}
