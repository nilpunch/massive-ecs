using System;
using System.Runtime.CompilerServices;
using Unity.IL2CPP.CompilerServices;

namespace Massive
{
	[Il2CppSetOption(Option.NullChecks, false)]
	[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
	public abstract class Allocator
	{
		public const int MaxPower = sizeof(int) * 8;
		public const int EndChunkId = int.MaxValue;

		public ChunkPagedArray Chunks { get; } = new ChunkPagedArray();

		public int ChunkCount { get; protected set; }

		public int[] ChunkFreeLists { get; } = new int[MaxPower + 1];

		public int UsedSpace { get; protected set; }

		public Allocator()
		{
			Array.Fill(ChunkFreeLists, EndChunkId);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public ChunkId Alloc(int minimumLength)
		{
			if (minimumLength < 0)
			{
				InvalidLengthException.Throw(minimumLength);
			}

			var chunkLength = MathUtils.NextPowerOf2(minimumLength);

			var freeList = MathUtils.FastLog2(chunkLength) + 1;

			var chunkId = ChunkFreeLists[freeList];
			if (chunkId != EndChunkId)
			{
				ref var chunk = ref Chunks[chunkId];
				ChunkFreeLists[freeList] = ~chunk.NextFreeId;
				chunk.Length = chunkLength;
				ResetData(chunk.Offset, chunkLength);
				return new ChunkId(chunkId, chunk.Version);
			}
			else
			{
				chunkId = ChunkCount;
				Chunks.EnsurePageAt(chunkId);
				ChunkCount += 1;

				var offset = UsedSpace;
				EnsureDataCapacity(offset + chunkLength);
				UsedSpace += chunkLength;

				ref var chunk = ref Chunks[chunkId];
				chunk.Offset = offset;
				chunk.Length = chunkLength;
				ResetData(offset, chunkLength);
				return new ChunkId(chunkId, chunk.Version);
			}
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
		public void Free(ChunkId chunkId)
		{
			if (chunkId.Id < 0 || chunkId.Id >= ChunkCount)
			{
				ChunkUnknownException.Throw(chunkId);
			}

			ref var chunk = ref Chunks[chunkId.Id];

			if (chunk.Length < 0 || chunk.Version != chunkId.Version)
			{
				ChunkUnknownException.Throw(chunkId);
			}

			var freeList = MathUtils.FastLog2(chunk.Length) + 1;
			chunk.NextFreeId = ~ChunkFreeLists[freeList];
			MathUtils.IncrementWrapTo1(ref chunk.Version);
			ChunkFreeLists[freeList] = chunkId.Id;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Resize(ChunkId chunkId, int minimumLength)
		{
			if (minimumLength < 0)
			{
				InvalidLengthException.Throw(minimumLength);
			}

			if (chunkId.Id < 0 || chunkId.Id >= ChunkCount)
			{
				ChunkUnknownException.Throw(chunkId);
			}

			ref var chunk = ref Chunks[chunkId.Id];

			if (chunk.Length < 0 || chunk.Version != chunkId.Version)
			{
				ChunkUnknownException.Throw(chunkId);
			}

			var goalLength = MathUtils.NextPowerOf2(minimumLength);

			if (chunk.Length == goalLength)
			{
				return;
			}

			var orignialFreeList = MathUtils.FastLog2(chunk.Length) + 1;
			var swapFreeList = MathUtils.FastLog2(goalLength) + 1;

			var swapId = ChunkFreeLists[swapFreeList];
			if (swapId != EndChunkId)
			{
				// Swap with existing free chunk of equal size.
				ref var swapChunk = ref Chunks[swapId];
				ChunkFreeLists[swapFreeList] = ~swapChunk.NextFreeId;

				CopyData(chunk.Offset, swapChunk.Offset, MathUtils.Min(chunk.Length, goalLength));
				if (goalLength > chunk.Length)
				{
					ResetData(swapChunk.Offset + chunk.Length, goalLength - chunk.Length);
				}

				(chunk.Offset, swapChunk.Offset) = (swapChunk.Offset, chunk.Offset);
				chunk.Length = goalLength;

				swapChunk.NextFreeId = ~ChunkFreeLists[orignialFreeList];
				ChunkFreeLists[orignialFreeList] = swapId;
			}
			else
			{
				// Create new chunk to swap with current one.
				swapId = ChunkCount;
				Chunks.EnsurePageAt(swapId);
				ChunkCount += 1;

				ref var swapChunk = ref Chunks[swapId];
				swapChunk.Offset = chunk.Offset;
				swapChunk.NextFreeId = ~ChunkFreeLists[orignialFreeList];
				ChunkFreeLists[orignialFreeList] = swapId;

				var offset = UsedSpace;
				EnsureDataCapacity(offset + goalLength);
				UsedSpace += goalLength;

				CopyData(chunk.Offset, offset, MathUtils.Min(chunk.Length, goalLength));
				if (goalLength > chunk.Length)
				{
					ResetData(offset + chunk.Length, goalLength - chunk.Length);
				}

				chunk.Offset = offset;
				chunk.Length = goalLength;
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public ref readonly Chunk GetChunk(ChunkId chunkId)
		{
			if (chunkId.Id < 0 || chunkId.Id >= ChunkCount)
			{
				ChunkUnknownException.Throw(chunkId);
			}

			ref var chunk = ref Chunks[chunkId.Id];

			if (chunk.Length < 0 || chunk.Version != chunkId.Version)
			{
				ChunkUnknownException.Throw(chunkId);
			}

			return ref chunk;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool IsValid(ChunkId chunkId)
		{
			if (chunkId.Id < 0 || chunkId.Id >= ChunkCount)
			{
				return false;
			}

			ref readonly var chunk = ref Chunks[chunkId.Id];

			return chunk.Length >= 0 && chunk.Version == chunkId.Version;
		}

		protected abstract void EnsureDataCapacity(int capacity);

		protected abstract void CopyData(int source, int destination, int length);

		protected abstract void ResetData(int start, int length);
	}

	[Il2CppSetOption(Option.NullChecks, false)]
	[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
	public sealed class Allocator<T> : Allocator
	{
		public T[] Data { get; private set; } = Array.Empty<T>();

		public int DataCapacity { get; private set; }

		protected override void EnsureDataCapacity(int capacity)
		{
			if (capacity > DataCapacity)
			{
				DataCapacity = MathUtils.NextPowerOf2(capacity);
				Data = Data.Resize(DataCapacity);
			}
		}

		protected override void CopyData(int source, int destination, int length)
		{
			Array.Copy(Data, source, Data, destination, length);
		}

		protected override void ResetData(int start, int length)
		{
			Array.Fill(Data, default, start, length);
		}

		public Allocator<T> Clone()
		{
			var clone = new Allocator<T>();
			CopyTo(clone);
			return clone;
		}

		public void CopyTo(Allocator<T> other)
		{
			MassiveAssert.EqualPageSize(Chunks, other.Chunks);

			other.EnsureDataCapacity(UsedSpace);

			Array.Copy(Data, other.Data, UsedSpace);
			Array.Copy(ChunkFreeLists, other.ChunkFreeLists, MaxPower + 1);
			other.ChunkCount = ChunkCount;
			other.UsedSpace = UsedSpace;

			var sourceChunks = Chunks;
			var destinationChunks = other.Chunks;

			foreach (var page in new PageSequence(sourceChunks.PageSize, ChunkCount))
			{
				destinationChunks.EnsurePage(page.Index);

				var sourcePage = sourceChunks.Pages[page.Index];
				var destinationPage = destinationChunks.Pages[page.Index];

				Array.Copy(sourcePage, destinationPage, page.Length);
			}
		}
	}
}
