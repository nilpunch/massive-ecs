using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Massive
{
	[StructLayout(LayoutKind.Sequential, Size = Pointer.Size, Pack = Pointer.Alignment)]
	public partial struct Pointer<T> where T : unmanaged
	{
		public Pointer AsPointer;

		public static Pointer Null => new Pointer()
		{
			AsInt = 0
		};

		public readonly bool IsNull
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => AsPointer.IsNull;
		}

		public readonly bool IsNotNull
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => AsPointer.IsNotNull;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static implicit operator Pointer(Pointer<T> pointer)
		{
			return pointer.AsPointer;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static unsafe implicit operator Pointer<T>(Pointer pointer)
		{
			return *(Pointer<T>*)(&pointer);
		}
	}
}
