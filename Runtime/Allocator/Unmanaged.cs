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

		static unsafe Unmanaged()
		{
			SizeInBytes = sizeof(T);
			Info = new UnmanagedInfo(SizeInBytes);
		}
	}

	public struct UnmanagedInfo
	{
		public readonly int Size;

		public UnmanagedInfo(int size)
		{
			Size = size;
		}
	}
}
