#if !MASSIVE_DISABLE_ASSERT
#define MASSIVE_ASSERT
#endif

using System.Runtime.CompilerServices;
using Unity.IL2CPP.CompilerServices;

namespace Massive
{
	[Il2CppSetOption(Option.NullChecks, false)]
	[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
	public readonly struct WorkablePointer<T> where T : unmanaged
	{
		private readonly Pointer<T> _pointer;
		private readonly Allocator _allocator;

		public WorkablePointer(Pointer<T> pointer, Allocator allocator)
		{
			_pointer = pointer;
			_allocator = allocator;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static implicit operator Pointer<T>(WorkablePointer<T> workablePointer)
		{
			return workablePointer._pointer;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Free()
		{
			_pointer.Free(_allocator);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void DeepFree()
		{
			_pointer.DeepFree(_allocator);
		}

		public ref T Value
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => ref _pointer.Value(_allocator);
		}

		public ref T this[int index]
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => ref _pointer.GetAt(_allocator, index);
		}
	}
}
