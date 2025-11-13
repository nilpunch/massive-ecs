#if !MASSIVE_DISABLE_ASSERT
#define MASSIVE_ASSERT
#endif

using System;
using System.Runtime.CompilerServices;

namespace Massive
{
	public unsafe partial struct ArrayPointer<T>
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public ref ArrayModel GetModel(Allocator allocator)
		{
			return ref *(ArrayModel*)(allocator.GetPage(ModelPointer.AsPointer).AlignedPtr + ModelPointer.AsPointer.Offset);
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
			AllocatorTypeSchema<ArrayModel<T>>.DeepFree(allocator, ModelPointer);
		}

		public ref T this[Allocator allocator, int index]
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				ref var model = ref GetModel(allocator);

				AllocatorIndexOutOfRangeException.ThrowIfOutOfRangeExclusive(index, model.Length);

				ref readonly var page = ref allocator.GetPage(model.Items);
				return ref ((T*)(page.AlignedPtr + model.Items.Offset))[index];
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public ref T GetAt(Allocator allocator, int index)
		{
			ref var model = ref GetModel(allocator);

			AllocatorIndexOutOfRangeException.ThrowIfOutOfRangeExclusive(index, model.Length);

			ref readonly var page = ref allocator.GetPage(model.Items);

			return ref ((T*)(page.AlignedPtr + model.Items.Offset))[index];
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public int Length(Allocator allocator)
		{
			return GetModel(allocator).Length;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Resize(Allocator allocator, int length, MemoryInit memoryInit = MemoryInit.Clear)
		{
			ref var model = ref GetModel(allocator);

			var info = Unmanaged<T>.Info;
			allocator.Resize(ref model.Items, length * info.Size, info.Alignment, memoryInit);
			model.Length = length;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public int IndexOf(Allocator allocator, T item)
		{
			throw new NotImplementedException();
			// return Array.IndexOf(Allocator.Data, item, Allocator.GetChunk(ChunkId).AlignedOffsetInBytes, Allocator.GetChunk(ChunkId).LengthInBytes);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public int IndexOf(Allocator allocator, T item, int startIndex, int count)
		{
			throw new NotImplementedException();

			// var chunk = Allocator.GetChunk(ChunkId);
			//
			// if ((startIndex + count) * Unmanaged<T>.SizeInBytes >= chunk.OffsetInBytes + chunk.LengthInBytes)
			// {
			// 	return -1;
			// }
			//
			// return Array.IndexOf(Allocator.Data, item, chunk.OffsetInBytes + startIndex, count);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void CopyTo(Allocator allocator, int sourceIndex, ArrayPointer<T> destinationArray, int destinationIndex, int length)
		{
			throw new NotImplementedException();
			// Array.Copy(Allocator.Data, Allocator.GetChunk(ChunkId).OffsetInBytes + sourceIndex * Unmanaged<T>.SizeInBytes,
			// 	destinationArray.Allocator.Data, destinationArray.Allocator.GetChunk(ChunkId).OffsetInBytes + destinationIndex * Unmanaged<T>.SizeInBytes,
			// 	length);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void CopyToSelf(Allocator allocator, int sourceIndex, int destinationIndex, int length)
		{
			ref var model = ref GetModel(allocator);

			ref readonly var dataPage = ref allocator.GetPage(model.Items);

			var lengthInBytes = length * Unmanaged<T>.SizeInBytes;
			var alignedChunkPtr = (T*)(dataPage.AlignedPtr + model.Items.Offset);
			UnsafeUtils.Copy(alignedChunkPtr + sourceIndex, alignedChunkPtr + destinationIndex, lengthInBytes);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void EnsureCapacity(Allocator allocator, int capacity)
		{
			ref var model = ref GetModel(allocator);

			if (capacity > model.Length)
			{
				var info = Unmanaged<T>.Info;
				allocator.Resize(ref model.Items, capacity * info.Size, info.Alignment);
				model.Length = capacity;
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public UnsafeEnumerator<T> GetEnumerator(Allocator allocator)
		{
			ref var model = ref GetModel(allocator);

			ref readonly var dataPage = ref allocator.GetPage(model.Items);

			UnsafeEnumerator<T> unsafeEnumerator = default;
			unsafeEnumerator.Data = (T*)(dataPage.AlignedPtr + model.Items.Offset);
			unsafeEnumerator.Length = model.Length;
			unsafeEnumerator.Index = -1;
			return unsafeEnumerator;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public UnsafeEnumerator<T> GetEnumerator(Allocator allocator, int length)
		{
			ref var model = ref GetModel(allocator);

			ref readonly var dataPage = ref allocator.GetPage(model.Items);

			UnsafeEnumerator<T> unsafeEnumerator = default;
			unsafeEnumerator.Data = (T*)(dataPage.AlignedPtr + model.Items.Offset);
			unsafeEnumerator.Length = length;
			unsafeEnumerator.Index = -1;
			return unsafeEnumerator;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public UnsafeEnumerator<T> GetEnumerator(Allocator allocator, int start, int length)
		{
			ref var model = ref GetModel(allocator);

			ref readonly var dataPage = ref allocator.GetPage(model.Items);

			UnsafeEnumerator<T> unsafeEnumerator = default;
			unsafeEnumerator.Data = (T*)(dataPage.AlignedPtr + model.Items.Offset);
			unsafeEnumerator.Length = length;
			unsafeEnumerator.Index = start - 1;
			return unsafeEnumerator;
		}
	}
}
