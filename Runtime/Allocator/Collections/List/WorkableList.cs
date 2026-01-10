#if !MASSIVE_DISABLE_ASSERT
#define MASSIVE_ASSERT
#endif

using System;
using System.Runtime.CompilerServices;
using Unity.IL2CPP.CompilerServices;

namespace Massive
{
	[Il2CppSetOption(Option.NullChecks, false)]
	[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
	public readonly struct WorkableList<T> where T : unmanaged
	{
		private readonly ListPointer<T> _list;
		private readonly Allocator _allocator;

		public WorkableList(ListPointer<T> list, Allocator allocator)
		{
			_list = list;
			_allocator = allocator;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static implicit operator ListPointer<T>(WorkableList<T> list)
		{
			return list._list;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Free()
		{
			_list.Free(_allocator);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void DeepFree()
		{
			_list.DeepFree(_allocator);
		}

		public ref T this[int index]
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => ref _list.Model.Value(_allocator)[_allocator, index];
		}

		public int Count
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => _list.Model.Value(_allocator).Count;
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			set => _list.Model.Value(_allocator).Count = value;
		}

		public int Capacity
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => _list.Model.Value(_allocator).Capacity;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Add(T item)
		{
			_list.Model.Value(_allocator).Add(_allocator, item);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Insert(int index, T item)
		{
			_list.Model.Value(_allocator).Insert(_allocator, index, item);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool Remove<U>(U item) where U : IEquatable<T>
		{
			return _list.Model.Value(_allocator).Remove(_allocator, item);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void RemoveAt(int index)
		{
			_list.Model.Value(_allocator).RemoveAt(_allocator, index);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void RemoveAtSwapBack(int index)
		{
			_list.Model.Value(_allocator).RemoveAtSwapBack(_allocator, index);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public int IndexOf<U>(U item) where U : IEquatable<T>
		{
			return _list.Model.Value(_allocator).IndexOf(_allocator, item);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Clear()
		{
			_list.Model.Value(_allocator).Clear();
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public UnsafeEnumerator<T> GetEnumerator()
		{
			return _list.Model.Value(_allocator).GetEnumerator(_allocator);
		}
	}
}
