using System.Runtime.InteropServices;

namespace Massive
{
	[StructLayout(LayoutKind.Sequential, Size = ArrayModel.Size, Pack = ArrayModel.Alignment)]
	public struct ArrayModel<T> where T : unmanaged
	{
		[PointerField(CountFieldName = nameof(Length))]
		public Pointer<T> Items;

		public int Length;
	}

	[StructLayout(LayoutKind.Sequential, Size = Size, Pack = Alignment)]
	public struct ArrayModel
	{
		public const int Size = Pointer.Size + sizeof(int);
		public const int Alignment = Size;

		public Pointer Items;

		public int Length;
	}
}
