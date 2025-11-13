#if !MASSIVE_DISABLE_ASSERT
#define MASSIVE_ASSERT
#endif

using System;
using System.Runtime.CompilerServices;

namespace Massive
{
	public readonly unsafe partial struct ListPointer<T>
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public ref ListModel GetModel(Allocator allocator)
		{
			return ref *(ListModel*)(allocator.GetPage(ModelPointer.AsPointer).AlignedPtr + ModelPointer.AsPointer.Offset);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Free(Allocator allocator)
		{
			allocator.Free(GetModel(allocator).Items);
			allocator.Free(ModelPointer.AsPointer);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void DeepFree(Allocator allocator)
		{
			AllocatorTypeSchema<ListModel<T>>.DeepFree(allocator, ModelPointer);
		}

		public ref T this[Allocator allocator, int index]
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				ref var model = ref GetModel(allocator);

				AllocatorIndexOutOfRangeException.ThrowIfOutOfRangeExclusive(index, model.Count);

				return ref AsPtr(allocator, model.Items)[index];
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public ref T GetAt(Allocator allocator, int index)
		{
			ref var model = ref GetModel(allocator);

			AllocatorIndexOutOfRangeException.ThrowIfOutOfRangeExclusive(index, model.Count);

			return ref AsPtr(allocator, model.Items)[index];
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public int Count(Allocator allocator)
		{
			return GetModel(allocator).Count;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public int Capacity(Allocator allocator)
		{
			return GetModel(allocator).Capacity;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Add(Allocator allocator, T item)
		{
			ref var model = ref GetModel(allocator);

			if (model.Count >= model.Capacity)
			{
				var newCapacity = MathUtils.NextPowerOf2(model.Count + 1);

				var info = Unmanaged<T>.Info;
				allocator.Resize(ref model.Items, newCapacity * info.Size, info.Alignment);
				model.Capacity = newCapacity;
			}

			AsPtr(allocator, model.Items)[model.Count++] = item;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool Remove(Allocator allocator, T item)
		{
			var index = IndexOf(allocator, item);
			if (index >= 0)
			{
				RemoveAt(allocator, index);
				return true;
			}

			return false;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Insert(Allocator allocator, int index, T item)
		{
			ref var model = ref GetModel(allocator);

			AllocatorIndexOutOfRangeException.ThrowIfOutOfRangeInclusive(index, model.Count);

			EnsureCapacityAt(allocator, model.Count);

			CopyToSelf(allocator, model.Items, index, index + 1, model.Count - index);
			AsPtr(allocator, model.Items)[index] = item;
			model.Count++;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void RemoveAt(Allocator allocator, int index)
		{
			ref var model = ref GetModel(allocator);

			AllocatorIndexOutOfRangeException.ThrowIfOutOfRangeExclusive(index, model.Count);

			model.Count--;
			CopyToSelf(allocator, model.Items, index + 1, index, model.Count - index);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void RemoveAtSwapBack(Allocator allocator, int index)
		{
			ref var model = ref GetModel(allocator);

			AllocatorIndexOutOfRangeException.ThrowIfOutOfRangeExclusive(index, model.Count);

			model.Count--;

			var items = AsPtr(allocator, model.Items);
			items[index] = items[model.Count];
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public int IndexOf(Allocator allocator, T item)
		{
			throw new NotImplementedException();
			// return ItemsId.IndexOf(allocator, item, 0, Count(allocator));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Clear(Allocator allocator)
		{
			GetModel(allocator).Count = 0;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void EnsureCapacityAt(Allocator allocator, int index)
		{
			ref var model = ref GetModel(allocator);

			if (index >= model.Capacity)
			{
				var newCapacity = MathUtils.NextPowerOf2(index + 1);

				var info = Unmanaged<T>.Info;
				allocator.Resize(ref model.Items, newCapacity * info.Size, info.Alignment);
				model.Capacity = newCapacity;
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public UnsafeEnumerator<T> GetEnumerator(Allocator allocator)
		{
			ref var model = ref GetModel(allocator);

			ref readonly var dataPage = ref allocator.GetPage(model.Items);

			UnsafeEnumerator<T> unsafeEnumerator = default;
			unsafeEnumerator.Data = (T*)(dataPage.AlignedPtr + model.Items.Offset);
			unsafeEnumerator.Length = model.Count;
			unsafeEnumerator.Index = -1;
			return unsafeEnumerator;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private T* AsPtr(Allocator allocator, Pointer pointer)
		{
			ref readonly var page = ref allocator.GetPage(pointer);

			return (T*)(page.AlignedPtr + pointer.Offset);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void CopyToSelf(Allocator allocator, Pointer pointer, int sourceIndex, int destinationIndex, int length)
		{
			ref readonly var dataPage = ref allocator.GetPage(pointer);
			var lengthInBytes = length * Unmanaged<T>.SizeInBytes;
			var alignedChunkPtr = (T*)(dataPage.AlignedPtr + pointer.Offset);
			UnsafeUtils.Copy(alignedChunkPtr + sourceIndex, alignedChunkPtr + destinationIndex, lengthInBytes);
		}
	}
}
