#if !MASSIVE_DISABLE_ASSERT
#define MASSIVE_ASSERT
#endif

using System.Runtime.CompilerServices;
using Unity.IL2CPP.CompilerServices;

namespace Massive
{
	[Il2CppSetOption(Option.NullChecks, false)]
	[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
	public readonly struct WorkableList<T> where T : unmanaged
	{
		private readonly ListHandle<T> _list;
		private readonly Allocator _allocator;

		public WorkableList(ListHandle<T> list, Allocator allocator)
		{
			_list = list;
			_allocator = allocator;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static implicit operator ListId(WorkableList<T> list)
		{
			return list._list;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static implicit operator ListHandle<T>(WorkableList<T> list)
		{
			return list._list;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Free()
		{
			_list.Free(_allocator);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public WorkableList<T> Track(int id)
		{
			_list.Track(_allocator, id);
			return this;
		}

		public ref T this[int index]
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => ref _list.GetAtUnchecked(_allocator, index);
		}

		public int Count
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => _list.Count(_allocator);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Add(T item)
		{
			_list.Add(_allocator, item);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool Remove(T item)
		{
			return _list.Remove(_allocator, item);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Insert(int index, T item)
		{
			_list.Insert(_allocator, index, item);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void RemoveAt(int index)
		{
			_list.RemoveAt(_allocator, index);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void RemoveAtSwapBack(int index)
		{
			_list.RemoveAtSwapBack(_allocator, index);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public int IndexOf(T item)
		{
			return _list.IndexOf(_allocator, item);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Clear()
		{
			_list.Clear(_allocator);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public UnsafeEnumerator<T> GetEnumerator()
		{
			return _list.Enumerate(_allocator);
		}
	}
}
