using System.Runtime.InteropServices;
using Unity.IL2CPP.CompilerServices;

namespace Massive
{
	[StructLayout(LayoutKind.Sequential, Size = ListModel.Size, Pack = ListModel.Alignment)]
	public struct ListModel<T> where T : unmanaged
	{
		[PointerField(CountFieldName = nameof(Count))]
		public Pointer<T> Items;

		public int Capacity;
		public int Count;
	}

	[StructLayout(LayoutKind.Sequential, Size = Size, Pack = Alignment)]
	public struct ListModel
	{
		public const int Size = Pointer.Size + sizeof(int) * 2;
		public const int Alignment = 16;

		public Pointer Items;

		public int Capacity;
		public int Count;
	}
}
