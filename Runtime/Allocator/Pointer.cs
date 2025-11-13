using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Massive
{
	[StructLayout(LayoutKind.Explicit, Size = Size, Pack = Alignment)]
	public struct Pointer
	{
		public const int Size = 4;
		public const int Alignment = 4;

		[FieldOffset(0)] public ushort Offset;

		[FieldOffset(2)] public ushort Page;

		[FieldOffset(0)] public int AsInt;

		public static Pointer Null => new Pointer()
		{
			AsInt = 0
		};

		public readonly bool IsNull
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => AsInt == 0;
		}

		public readonly bool IsNotNull
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => AsInt != 0;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static unsafe explicit operator Pointer(int pointerAsInt)
		{
			return *(Pointer*)(&pointerAsInt);
		}
	}
}
