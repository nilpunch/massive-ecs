using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace Massive.Tests
{
	[TestFixture]
	public class AllocatorTests
	{
		private const int Iterations = 100_000;
		private const int MaxAllocedPages = 128;
		private const int Seed = 1337;

		private static readonly int[] Sizes = new[] { 0, 1, 2, 4, 8, 16, 32, 64, 128, 256, 10_000, 100_000 };

		private Allocator _allocator;
		private List<(Pointer pointer, int size)> _activePointers;

		[SetUp]
		public void SetUp()
		{
			_allocator = new Allocator();
			_activePointers = new List<(Pointer pointer, int size)>();
			var rnd = new Random(Seed);

			for (var i = 0; i < Iterations; ++i)
			{
				var action = rnd.Next(3);

				switch (action)
				{
					case 0: // Alloc
					{
						var size = Sizes[rnd.Next(Sizes.Length)];
						var pointer = _allocator.Alloc(size, 1);
						Assert.Greater(pointer.AsInt, 0);
						Assert.IsTrue(_allocator.IsAllocated(pointer));
						_activePointers.Add((pointer, size));
						break;
					}

					case 1: // Free
						if (_activePointers.Count > 0)
						{
							var index = rnd.Next(_activePointers.Count);
							var pointer = _activePointers[index];
							_allocator.Free(pointer.pointer);
							Assert.IsFalse(_allocator.IsAllocated(pointer.pointer));
							_activePointers.RemoveAt(index);
						}
						break;

					case 2: // Resize
						if (_activePointers.Count > 0)
						{
							var index = rnd.Next(_activePointers.Count);
							var pointer = _activePointers[index];
							var newSize = Sizes[rnd.Next(Sizes.Length)];
							_allocator.Resize(ref pointer.pointer, newSize, 1);
							Assert.IsTrue(_allocator.IsAllocated(pointer.pointer));
							_activePointers[index] = (pointer.pointer, newSize);
						}
						break;
				}
			}

			// Final cleanup
			foreach (var (pointer, _) in _activePointers)
				_allocator.Free(pointer);

			_activePointers.Clear();
		}

		[Test]
		public void AllSlotsShouldBeInFreelist()
		{
			for (var slotClassIndex = 0; slotClassIndex < Allocator.AllClassCount; slotClassIndex++)
			{
				var slotClass = slotClassIndex + Allocator.MinSlotClass;
				var freeSlotsCount = 0;
				var current = _allocator.FreeToAlloc[slotClassIndex];
				while (current.IsNotNull)
				{
					freeSlotsCount++;
					current = _allocator.ValueUnsafe<Pointer>(current);
				}

				var usedBytes = 0;
				for (var index = 0; index < _allocator.PageCount; index++)
				{
					var page = _allocator.Pages[index];
					if (page.SlotClass == slotClass)
					{
						usedBytes += page.PageLength;
					}
				}

				if (_allocator.NextToAlloc[slotClassIndex].Offset != 0)
				{
					usedBytes -= Allocator.PageLength(slotClass);
					usedBytes += _allocator.NextToAlloc[slotClassIndex].Offset;
				}

				var usedSlotCount = usedBytes / (1 << slotClass);
				var reservedSlotCorrection = slotClassIndex == 0 ? -1 : 0;
				Assert.AreEqual(usedSlotCount + reservedSlotCorrection, freeSlotsCount, $"All pages of class {slotClass} must be in freelist");
			}
		}

		[Test]
		public void PageCountShouldBeReasonable()
		{
			Assert.LessOrEqual(_allocator.PageCount, MaxAllocedPages, "Page count unexpectedly high (reuse failed?)");
		}

		[Test]
		public void PageLengthShouldBeValid()
		{
			Assert.LessOrEqual(_allocator.PageCount, _allocator.Pages.Length);
		}
	}
}
