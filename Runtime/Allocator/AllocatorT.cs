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
	public sealed class Allocator<T> : Allocator where T : unmanaged
	{
		public T DefaultValue { get; }

		public T[] Data { get; private set; } = Array.Empty<T>();

		private int DataCapacity { get; set; }

		public Allocator(T defaultValue = default)
			: base(AllocatorId<T>.Index)
		{
			DefaultValue = defaultValue;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public new ChunkId Alloc(int minimumLength, MemoryInit memoryInit = MemoryInit.Clear)
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
					ClearDataInternal(chunk.Offset, chunkLength);
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
				EnsureDataCapacityInternal(offset + chunkLength);
				UsedSpace += chunkLength;

				ref var chunk = ref Chunks[chunkId];
				chunk.Offset = offset;
				chunk.Length = chunkLength;
				if (chunkLength != 0 && memoryInit == MemoryInit.Clear)
				{
					ClearDataInternal(offset, chunkLength);
				}
				return new ChunkId(chunkId, chunk.Version);
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public new void Resize(ChunkId chunkId, int minimumLength, MemoryInit memoryInit = MemoryInit.Clear)
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

				CopyDataInternal(chunk.Offset, swapChunk.Offset, MathUtils.Min(chunk.Length, newLength));
				if (newLength > chunk.Length && memoryInit == MemoryInit.Clear)
				{
					ClearDataInternal(swapChunk.Offset + chunk.Length, newLength - chunk.Length);
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
				EnsureDataCapacityInternal(offset + newLength);
				UsedSpace += newLength;

				CopyDataInternal(chunk.Offset, offset, MathUtils.Min(chunk.Length, newLength));
				if (newLength > chunk.Length && memoryInit == MemoryInit.Clear)
				{
					ClearDataInternal(offset + chunk.Length, newLength - chunk.Length);
				}

				chunk.Offset = offset;
				chunk.Length = newLength;
			}
		}

		public override void EnsureDataCapacity(int capacity)
		{
			EnsureDataCapacityInternal(capacity);
		}

		protected override void CopyData(int source, int destination, int length)
		{
			CopyDataInternal(source, destination, length);
		}

		protected override void ClearData(int start, int length)
		{
			ClearDataInternal(start, length);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void EnsureDataCapacityInternal(int capacity)
		{
			if (capacity > DataCapacity)
			{
				DataCapacity = MathUtils.NextPowerOf2(capacity);
				Data = Data.Resize(DataCapacity);
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void CopyDataInternal(int source, int destination, int length)
		{
			if (length > 4)
			{
				Array.Copy(Data, source, Data, destination, length);
				return;
			}

			for (var i = 0; i < length; i++)
			{
				Data[destination + i] = Data[source + i];
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void ClearDataInternal(int start, int length)
		{
			if (length > 4)
			{
				Array.Fill(Data, DefaultValue, start, length);
				return;
			}

			for (var i = 0; i < length; i++)
			{
				Data[start + i] = DefaultValue;
			}
		}

		public override Type ElementType => typeof(T);

		public override Array RawData => Data;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Allocator<T> Clone()
		{
			var clone = new Allocator<T>();
			CopyTo(clone);
			return clone;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void CopyTo(Allocator<T> other)
		{
			other.EnsureDataCapacity(UsedSpace);

			Array.Copy(Chunks, other.Chunks, ChunkCount);
			Array.Copy(ChunkFreeLists, other.ChunkFreeLists, FreeListsLength);
			Array.Copy(Data, other.Data, UsedSpace);

			if (ChunkCount < other.ChunkCount)
			{
				Array.Fill(other.Chunks, Chunk.DefaultValid, ChunkCount, other.ChunkCount - ChunkCount);
			}

			other.SetState(ChunkCount, UsedSpace);
		}
	}
}
