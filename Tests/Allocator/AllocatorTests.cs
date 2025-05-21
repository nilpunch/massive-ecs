using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace Massive.Tests
{
	[TestFixture]
	public class AllocatorTests
	{
		private const int Iterations = 100_000;
		private const int MaxActiveChunks = 1024;
		private const int Seed = 1337;

		private static readonly int[] Sizes = new[] { 0, 1, 2, 4, 8, 16, 32, 64, 128 };

		private Allocator<int> _allocator;
		private List<(ChunkId chunkId, int size)> _activeChunks;

		[SetUp]
		public void SetUp()
		{
			_allocator = new Allocator<int>();
			_activeChunks = new List<(ChunkId chunkId, int size)>();
			var rnd = new Random(Seed);

			for (var i = 0; i < Iterations; ++i)
			{
				var action = rnd.Next(3);

				switch (action)
				{
					case 0: // Alloc
						if (_activeChunks.Count < MaxActiveChunks)
						{
							var size = Sizes[rnd.Next(Sizes.Length)];
							var chunkId = _allocator.Alloc(size);
							Assert.GreaterOrEqual(chunkId.Id, 0);
							Assert.GreaterOrEqual(chunkId.Version, 1U);
							_activeChunks.Add((chunkId, size));
						}
						break;

					case 1: // Free
						if (_activeChunks.Count > 0)
						{
							var index = rnd.Next(_activeChunks.Count);
							var chunk = _activeChunks[index];
							_allocator.Free(chunk.chunkId);
							_activeChunks.RemoveAt(index);
						}
						break;

					case 2: // Resize
						if (_activeChunks.Count > 0)
						{
							var index = rnd.Next(_activeChunks.Count);
							var chunk = _activeChunks[index];
							var newSize = Sizes[rnd.Next(Sizes.Length)];
							_allocator.Resize(chunk.chunkId, newSize);
							_activeChunks[index] = (chunk.chunkId, newSize);
						}
						break;
				}
			}

			// Final cleanup
			foreach (var (chunkId, _) in _activeChunks)
				_allocator.Free(chunkId);

			_activeChunks.Clear();
		}

		[Test]
		public void AllChunksShouldBeInFreelist()
		{
			var freelistCount = 0;
			foreach (var head in _allocator.ChunkFreeLists)
			{
				var current = head;
				while (current != Allocator.FreeListEndId)
				{
					freelistCount++;
					current = ~_allocator.Chunks[current].NextFreeId;
				}
			}

			Assert.AreEqual(_allocator.ChunkCount, freelistCount, "All chunks must be in freelist");
		}

		[Test]
		public void FreelistShouldHaveCorrectTailCount()
		{
			var usedFreeLists = _allocator.ChunkFreeLists.Count(h => h != Allocator.FreeListEndId);

			var tailCount = 0;
			for (var i = 0; i < _allocator.ChunkCount; i++)
			{
				if (_allocator.Chunks[i].NextFreeId == ~Allocator.FreeListEndId)
					tailCount++;
			}

			Assert.AreEqual(tailCount, usedFreeLists, "Tail chunk count must match used free list count");
		}

		[Test]
		public void ChunkCountShouldBeReasonable()
		{
			Assert.LessOrEqual(_allocator.ChunkCount, MaxActiveChunks, "Chunk count unexpectedly high (reuse failed?)");
		}

		[Test]
		public void ChunkOffsetsShouldBeWithinBounds()
		{
			for (var i = 0; i < _allocator.ChunkCount; ++i)
			{
				ref var c = ref _allocator.Chunks[i];
				Assert.LessOrEqual(c.Offset + c.Length, _allocator.UsedSpace);
			}
		}

		[Test]
		public void DataLengthShouldBeValid()
		{
			Assert.LessOrEqual(_allocator.UsedSpace, _allocator.Data.Length);
		}
	}
}
