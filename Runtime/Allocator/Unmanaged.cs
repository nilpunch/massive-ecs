using System.Runtime.InteropServices;
using Unity.IL2CPP.CompilerServices;

// ReSharper disable StaticMemberInGenericType
namespace Massive
{
	[Il2CppEagerStaticClassConstruction]
	[Il2CppSetOption(Option.NullChecks, false)]
	[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
	public static class Unmanaged<T> where T : unmanaged
	{
		public static readonly UnmanagedInfo Info;
		public static readonly int SizeInBytes;
		public static readonly int Alignment;

		static unsafe Unmanaged()
		{
			SizeInBytes = sizeof(T);
			Alignment = (int)Marshal.OffsetOf<AlignmentHelper>(nameof(AlignmentHelper.Target));
			Info = new UnmanagedInfo(SizeInBytes, Alignment);
		}

		private struct AlignmentHelper
		{
			public byte Padding;
			public T Target;
		}
	}

	public struct UnmanagedInfo
	{
		public readonly int Size;
		public readonly int Alignment;

		public UnmanagedInfo(int size, int alignment)
		{
			Size = size;
			Alignment = alignment;
		}
	}
}
