#if !MASSIVE_DISABLE_ASSERT
#define MASSIVE_ASSERT
#endif

using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Unity.IL2CPP.CompilerServices;

namespace Massive
{
	[Il2CppSetOption(Option.NullChecks, false)]
	[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
	public unsafe partial class Allocator
	{
		private const int FreeListsLength = 1 + sizeof(int) * 8;
		public const int FreeListEndId = int.MaxValue;

		public Chunk[] Chunks { get; private set; } = Array.Empty<Chunk>();

		private int ChunksCapacity { get; set; }

		public int ChunkCount { get; private set; }

		public int[] ChunkFreeLists { get; } = new int[FreeListsLength];

		public int UsedSpace { get; private set; }

		public byte[] Data { get; private set; } = Array.Empty<byte>();

		public byte* AlignedPtr { get; private set; }

		private int BaseDataAlignment { get; set; }

		private GCHandle DataHandle { get; set; }

		private int DataCapacity { get; set; }

		public Allocator(int baseDataAlginement = 64)
		{
			NotPowerOfTwoArgumentException.ThrowIfNotPowerOfTwo(baseDataAlginement);

			BaseDataAlignment = baseDataAlginement;
			Array.Fill(ChunkFreeLists, FreeListEndId);

			DataHandle = GCHandle.Alloc(Data, GCHandleType.Pinned);
			AlignedPtr = (byte*)DataHandle.AddrOfPinnedObject().ToPointer();
		}

		~Allocator()
		{
			DataHandle.Free();
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public ref T Get<T>(ChunkId chunkId) where T : unmanaged
		{
			ChunkNotFoundException.ThrowIfNotInCountRange(this, chunkId);

			ref var chunk = ref Chunks[chunkId.Id];

			ChunkNotFoundException.ThrowIfDeallocated(chunk, chunkId);

			return ref *(T*)(AlignedPtr + chunk.AlignedOffsetInBytes);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public ref T GetAt<T>(ChunkId chunkId, int index) where T : unmanaged
		{
			ChunkNotFoundException.ThrowIfNotInCountRange(this, chunkId);

			ref var chunk = ref Chunks[chunkId.Id];

			ChunkNotFoundException.ThrowIfDeallocated(chunk, chunkId);
			AllocatorIndexOutOfRangeException.ThrowIfOutOfRangeExclusive(index * Unmanaged<T>.SizeInBytes, Chunks[chunkId.Id].LengthInBytes);

			return ref ((T*)(AlignedPtr + chunk.AlignedOffsetInBytes))[index];
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public ChunkId Alloc<T>(int minimumLength, MemoryInit memoryInit = MemoryInit.Clear) where T : unmanaged
		{
			var info = Unmanaged<T>.Info;
			return Alloc(minimumLength * info.Size, info.Alignment, memoryInit);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public ChunkId Alloc(int minimumLength, int alignment, MemoryInit memoryInit = MemoryInit.Clear)
		{
			NegativeArgumentException.ThrowIfNegative(minimumLength);
			NotPowerOfTwoArgumentException.ThrowIfNotPowerOfTwo(alignment);

			minimumLength += alignment;

			MathUtils.NextPowerOf2AndLog2(minimumLength, out var chunkLength, out var freeList);
			freeList += 1;

			var chunkId = ChunkFreeLists[freeList];
			if (chunkId != FreeListEndId)
			{
				// Reuse existing free chunk of equal size.
				ref var chunk = ref Chunks[chunkId];
				ChunkFreeLists[freeList] = ~chunk.NextFreeId;
				chunk.LengthInBytes = chunkLength;
				if (chunkLength != 0 && memoryInit == MemoryInit.Clear)
				{
					ClearData(chunk.OffsetInBytes, chunkLength);
				}
				chunk.AlignedOffsetInBytes = (chunk.OffsetInBytes + alignment - 1) & -alignment;
				return new ChunkId(chunkId, chunk.Version);
			}
			else
			{
				if (ChunkFreeLists[0] != FreeListEndId)
				{
					// Reuse 0-length free chunk.
					chunkId = ChunkFreeLists[0];
					ChunkFreeLists[0] = ~Chunks[chunkId].NextFreeId;
				}
				else
				{
					// Create new chunk.
					chunkId = ChunkCount;
					EnsureChunkAt(chunkId);
					ChunkCount += 1;
				}

				var offset = UsedSpace;
				EnsureDataCapacity(offset + chunkLength);
				UsedSpace += chunkLength;

				ref var chunk = ref Chunks[chunkId];
				chunk.OffsetInBytes = offset;
				chunk.LengthInBytes = chunkLength;
				chunk.AlignedOffsetInBytes = (chunk.OffsetInBytes + alignment - 1) & -alignment;
				if (chunkLength != 0 && memoryInit == MemoryInit.Clear)
				{
					ClearData(offset, chunkLength);
				}
				return new ChunkId(chunkId, chunk.Version);
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Resize<T>(ChunkId chunkId, int minimumLength, MemoryInit memoryInit = MemoryInit.Clear) where T : unmanaged
		{
			var info = Unmanaged<T>.Info;
			Resize(chunkId, minimumLength * info.Size, info.Alignment, memoryInit);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Resize(ChunkId chunkId, int minimumLength, int alignment, MemoryInit memoryInit = MemoryInit.Clear)
		{
			ChunkNotFoundException.ThrowIfNotInCountRange(this, chunkId);
			NotPowerOfTwoArgumentException.ThrowIfNotPowerOfTwo(alignment);

			minimumLength += alignment;

			ref var chunk = ref Chunks[chunkId.Id];

			ChunkNotFoundException.ThrowIfDeallocated(chunk, chunkId);

			var newLength = MathUtils.NextPowerOf2(minimumLength);

			if (chunk.LengthInBytes == newLength)
			{
				// Just ensure alignment.
				chunk.AlignedOffsetInBytes = (chunk.OffsetInBytes + alignment - 1) & -alignment;
				return;
			}

			var originalFreeList = MathUtils.FastLog2(chunk.LengthInBytes) + 1;
			var swapFreeList = MathUtils.FastLog2(newLength) + 1;

			var swapId = ChunkFreeLists[swapFreeList];
			if (swapId != FreeListEndId)
			{
				// Swap with existing free chunk of equal size.
				ref var swapChunk = ref Chunks[swapId];
				ChunkFreeLists[swapFreeList] = ~swapChunk.NextFreeId;

				CopyData(chunk.OffsetInBytes, swapChunk.OffsetInBytes, MathUtils.Min(chunk.LengthInBytes, newLength));
				if (newLength > chunk.LengthInBytes && memoryInit == MemoryInit.Clear)
				{
					ClearData(swapChunk.OffsetInBytes + chunk.LengthInBytes, newLength - chunk.LengthInBytes);
				}

				(chunk.OffsetInBytes, swapChunk.OffsetInBytes) = (swapChunk.OffsetInBytes, chunk.OffsetInBytes);
				chunk.LengthInBytes = newLength;
				chunk.AlignedOffsetInBytes = (chunk.OffsetInBytes + alignment - 1) & -alignment;

				swapChunk.NextFreeId = ~ChunkFreeLists[originalFreeList];
				ChunkFreeLists[originalFreeList] = swapId;
			}
			else
			{
				if (ChunkFreeLists[0] != FreeListEndId)
				{
					// Reuse 0-length free chunk to swap with current one.
					swapId = ChunkFreeLists[0];
					ChunkFreeLists[0] = ~Chunks[swapId].NextFreeId;
				}
				else
				{
					// Create new chunk to swap with current one.
					swapId = ChunkCount;
					EnsureChunkAt(swapId);
					ChunkCount += 1;
					chunk = ref Chunks[chunkId.Id]; // Revalidate reference after resize.
				}

				ref var swapChunk = ref Chunks[swapId];
				swapChunk.OffsetInBytes = chunk.OffsetInBytes;
				swapChunk.NextFreeId = ~ChunkFreeLists[originalFreeList];
				ChunkFreeLists[originalFreeList] = swapId;

				var offset = UsedSpace;
				EnsureDataCapacity(offset + newLength);
				UsedSpace += newLength;

				CopyData(chunk.OffsetInBytes, offset, MathUtils.Min(chunk.LengthInBytes, newLength));
				if (newLength > chunk.LengthInBytes && memoryInit == MemoryInit.Clear)
				{
					ClearData(offset + chunk.LengthInBytes, newLength - chunk.LengthInBytes);
				}

				chunk.OffsetInBytes = offset;
				chunk.LengthInBytes = newLength;
				chunk.AlignedOffsetInBytes = (chunk.OffsetInBytes + alignment - 1) & -alignment;
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Free(ChunkId chunkId)
		{
			ChunkNotFoundException.ThrowIfNotInCountRange(this, chunkId);

			ref var chunk = ref Chunks[chunkId.Id];

			ChunkNotFoundException.ThrowIfDeallocated(chunk, chunkId);

			var freeList = MathUtils.FastLog2(chunk.LengthInBytes) + 1;
			chunk.NextFreeId = ~ChunkFreeLists[freeList];
			MathUtils.IncrementWrapTo1(ref chunk.Version);
			ChunkFreeLists[freeList] = chunkId.Id;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool TryFree(ChunkId chunkId)
		{
			if (chunkId.Id < 0 || chunkId.Id >= ChunkCount)
			{
				return false;
			}

			ref var chunk = ref Chunks[chunkId.Id];

			if (chunk.LengthInBytes < 0 || chunk.Version != chunkId.Version)
			{
				return false;
			}

			var freeList = MathUtils.FastLog2(chunk.LengthInBytes) + 1;
			chunk.NextFreeId = ~ChunkFreeLists[freeList];
			MathUtils.IncrementWrapTo1(ref chunk.Version);
			ChunkFreeLists[freeList] = chunkId.Id;

			return true;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public ref Chunk GetChunk(ChunkId chunkId)
		{
			ChunkNotFoundException.ThrowIfNotInCountRange(this, chunkId);

			ref var chunk = ref Chunks[chunkId.Id];

			ChunkNotFoundException.ThrowIfDeallocated(chunk, chunkId);

			return ref chunk;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool IsAllocated(ChunkId chunkId)
		{
			if (chunkId.Id < 0 || chunkId.Id >= ChunkCount)
			{
				return false;
			}

			ref var chunk = ref Chunks[chunkId.Id];

			return chunk.LengthInBytes >= 0 && chunk.Version == chunkId.Version;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Reset()
		{
			Array.Fill(ChunkFreeLists, FreeListEndId);
			Array.Fill(Chunks, Chunk.DefaultValid, 0, ChunkCount);

			UsedSpace = 0;
			ChunkCount = 0;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void EnsureChunkAt(int index)
		{
			if (index >= ChunksCapacity)
			{
				var newCapacity = MathUtils.NextPowerOf2(index + 1);

				Chunks = Chunks.Resize(newCapacity);
				Array.Fill(Chunks, Chunk.DefaultValid, ChunksCapacity, newCapacity - ChunksCapacity);
				ChunksCapacity = newCapacity;
			}
		}

		/// <summary>
		/// Sets the current state for serialization or rollback purposes.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void SetState(int chunksCount, int usedSpace)
		{
			ChunkCount = chunksCount;
			UsedSpace = usedSpace;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void EnsureDataCapacity(int capacity)
		{
			if (capacity > DataCapacity)
			{
				var prevCapacity = DataCapacity;
				var prevAlignedData = AlignedPtr;
				var prevDataHandle = DataHandle;

				DataCapacity = MathUtils.NextPowerOf2(capacity);
				Data = new byte[DataCapacity + BaseDataAlignment - 1];

				DataHandle = GCHandle.Alloc(Data, GCHandleType.Pinned);
				var addr = (long)DataHandle.AddrOfPinnedObject();
				var alignedAddr = (addr + (BaseDataAlignment - 1)) & -BaseDataAlignment;
				AlignedPtr = (byte*)alignedAddr;

				// Copy using aligned offsets.
				Buffer.MemoryCopy(prevAlignedData, AlignedPtr, DataCapacity, prevCapacity);

				prevDataHandle.Free();
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void CopyData(int source, int destination, int length)
		{
			var sourcePtr = AlignedPtr + source;
			var destinationPtr = AlignedPtr + destination;

			if (length > 128)
			{
				Buffer.MemoryCopy(sourcePtr, destinationPtr, length, length);
				return;
			}

			var sourcePtrLong = (long*)sourcePtr;
			var destinationPtrLong = (long*)destinationPtr;

			var lengthLong = length >> 3;

			for (var i = 0; i < lengthLong; i++)
			{
				destinationPtrLong[i] = sourcePtrLong[i];
			}

			// Reminder.
			for (var i = lengthLong << 3; i < length; i++)
			{
				destinationPtr[i] = sourcePtr[i];
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void ClearData(int start, int length)
		{
			var alignedPtr = AlignedPtr + start;
			var alignedPtrLong = (long*)alignedPtr;

			var lengthLong = length >> 3;

			for (var i = 0; i < lengthLong; i++)
			{
				alignedPtrLong[i] = default;
			}

			// Reminder.
			for (var i = lengthLong << 3; i < length; i++)
			{
				alignedPtr[i] = default;
			}
		}

		public Allocator Clone()
		{
			var clone = new Allocator();
			CopyTo(clone);
			return clone;
		}

		public void CopyTo(Allocator other)
		{
			other.EnsureDataCapacity(UsedSpace);

			Array.Copy(Chunks, other.Chunks, ChunkCount);
			Array.Copy(ChunkFreeLists, other.ChunkFreeLists, FreeListsLength);
			Buffer.MemoryCopy(AlignedPtr, other.AlignedPtr, UsedSpace, UsedSpace);

			if (ChunkCount < other.ChunkCount)
			{
				Array.Fill(other.Chunks, Chunk.DefaultValid, ChunkCount, other.ChunkCount - ChunkCount);
			}

			other.SetState(ChunkCount, UsedSpace);
		}
	}
}
