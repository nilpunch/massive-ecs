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
	public abstract class Allocator
	{
		protected const int FreeListsLength = 1 + sizeof(int) * 8;
		public const int FreeListEndId = int.MaxValue;

		/// <summary>
		/// Non-deterministic, used for lookups.<br/>
		/// Don't store it in simulation.
		/// </summary>
		public int AllocatorId { get; }

		public Chunk[] Chunks { get; private set; } = Array.Empty<Chunk>();

		private int ChunkCapacity { get; set; }

		public int ChunkCount { get; private set; }

		public int[] ChunkFreeLists { get; } = new int[FreeListsLength];

		public int UsedSpace { get; private set; }

		protected Allocator(int allocatorId)
		{
			AllocatorId = allocatorId;
			Array.Fill(ChunkFreeLists, FreeListEndId);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public ChunkId Alloc(int minimumLength, MemoryInit memoryInit = MemoryInit.Clear)
		{
			NegativeArgumentException.ThrowIfNegative(minimumLength);

			var chunkLength = MathUtils.NextPowerOf2(minimumLength);
			var freeList = MathUtils.FastLog2(chunkLength) + 1;

			var chunkId = ChunkFreeLists[freeList];
			if (chunkId != FreeListEndId)
			{
				// Reuse existing free chunk of equal size.
				ref var chunk = ref Chunks[chunkId];
				ChunkFreeLists[freeList] = ~chunk.NextFreeId;
				chunk.Length = chunkLength;
				if (chunkLength != 0 && memoryInit == MemoryInit.Clear)
				{
					ClearData(chunk.Offset, chunkLength);
				}
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
				chunk.Offset = offset;
				chunk.Length = chunkLength;
				if (chunkLength != 0 && memoryInit == MemoryInit.Clear)
				{
					ClearData(offset, chunkLength);
				}
				return new ChunkId(chunkId, chunk.Version);
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Resize(ChunkId chunkId, int minimumLength, MemoryInit memoryInit = MemoryInit.Clear)
		{
			ChunkNotFoundException.ThrowIfNotInCountRange(this, chunkId);

			ref var chunk = ref Chunks[chunkId.Id];

			ChunkNotFoundException.ThrowIfDeallocated(chunk, chunkId);

			var newLength = MathUtils.NextPowerOf2(minimumLength);

			if (chunk.Length == newLength)
			{
				return;
			}

			var originalFreeList = MathUtils.FastLog2(chunk.Length) + 1;
			var swapFreeList = MathUtils.FastLog2(newLength) + 1;

			var swapId = ChunkFreeLists[swapFreeList];
			if (swapId != FreeListEndId)
			{
				// Swap with existing free chunk of equal size.
				ref var swapChunk = ref Chunks[swapId];
				ChunkFreeLists[swapFreeList] = ~swapChunk.NextFreeId;

				CopyData(chunk.Offset, swapChunk.Offset, MathUtils.Min(chunk.Length, newLength));
				if (newLength > chunk.Length && memoryInit == MemoryInit.Clear)
				{
					ClearData(swapChunk.Offset + chunk.Length, newLength - chunk.Length);
				}

				(chunk.Offset, swapChunk.Offset) = (swapChunk.Offset, chunk.Offset);
				chunk.Length = newLength;

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
				swapChunk.Offset = chunk.Offset;
				swapChunk.NextFreeId = ~ChunkFreeLists[originalFreeList];
				ChunkFreeLists[originalFreeList] = swapId;

				var offset = UsedSpace;
				EnsureDataCapacity(offset + newLength);
				UsedSpace += newLength;

				CopyData(chunk.Offset, offset, MathUtils.Min(chunk.Length, newLength));
				if (newLength > chunk.Length && memoryInit == MemoryInit.Clear)
				{
					ClearData(offset + chunk.Length, newLength - chunk.Length);
				}

				chunk.Offset = offset;
				chunk.Length = newLength;
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Free(AllocatorChunkId allocatorChunkId)
		{
			ChunkNotFoundException.ThrowIfFromOtherAllocator(this, allocatorChunkId);

			Free(allocatorChunkId.ChunkId);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Free(ChunkId chunkId)
		{
			ChunkNotFoundException.ThrowIfNotInCountRange(this, chunkId);

			ref var chunk = ref Chunks[chunkId.Id];

			ChunkNotFoundException.ThrowIfDeallocated(chunk, chunkId);

			var freeList = MathUtils.FastLog2(chunk.Length) + 1;
			chunk.NextFreeId = ~ChunkFreeLists[freeList];
			MathUtils.IncrementWrapTo1(ref chunk.Version);
			ChunkFreeLists[freeList] = chunkId.Id;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool TryFree(AllocatorChunkId allocatorChunkId)
		{
			ChunkNotFoundException.ThrowIfFromOtherAllocator(this, allocatorChunkId);

			return TryFree(allocatorChunkId.ChunkId);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool TryFree(ChunkId chunkId)
		{
			if (chunkId.Id < 0 || chunkId.Id >= ChunkCount)
			{
				return false;
			}

			ref var chunk = ref Chunks[chunkId.Id];

			if (chunk.Length < 0 || chunk.Version != chunkId.Version)
			{
				return false;
			}

			var freeList = MathUtils.FastLog2(chunk.Length) + 1;
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

			return chunk.Length >= 0 && chunk.Version == chunkId.Version;
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
			if (index >= ChunkCapacity)
			{
				var newCapacity = MathUtils.NextPowerOf2(index + 1);

				Chunks = Chunks.Resize(newCapacity);
				Array.Fill(Chunks, Chunk.DefaultValid, ChunkCapacity, newCapacity - ChunkCapacity);
				ChunkCapacity = newCapacity;
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

		public abstract Type ElementType { get; }

		public abstract Array RawData { get; }

		public abstract void EnsureDataCapacity(int capacity);

		protected abstract void CopyData(int source, int destination, int length);

		protected abstract void ClearData(int start, int length);
	}
}
