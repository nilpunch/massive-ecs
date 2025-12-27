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
	public unsafe class Allocator
	{
		public readonly struct Page
		{
			public readonly byte* AlignedPtr;

			public readonly uint* UsedSlots;

			public readonly int SlotClass;

			public int SlotLength
			{
				[MethodImpl(MethodImplOptions.AggressiveInlining)]
				get => 1 << SlotClass;
			}

			public int PageLength
			{
				[MethodImpl(MethodImplOptions.AggressiveInlining)]
				get => PageLength(SlotClass);
			}

			public int BitSetLength
			{
				[MethodImpl(MethodImplOptions.AggressiveInlining)]
				get => BitSetLength(SlotClass);
			}

			public int PageLengthWithBitset
			{
				[MethodImpl(MethodImplOptions.AggressiveInlining)]
				get => PageLength + BitSetLength;
			}

			public Page(byte* alignedPtr, int slotClass)
			{
				AlignedPtr = alignedPtr;
				SlotClass = slotClass;
				UsedSlots = (uint*)(alignedPtr + PageLength(slotClass));
			}
		}

		public const int MinPageSlotClass = 16;
		public const int MinPageLength = 1 << MinPageSlotClass;
		public const int MinSlotClass = 2; // Ensures alignment for free list of Pointer's.
		public const int MaxSlotClass = 32;
		public const int MinSlotLength = 1 << MinSlotClass;
		public const int AllClassCount = MaxSlotClass - MinSlotClass;

		public const uint SingleBit = 1U;
		public const int SlotPower = 5;
		public const int SlotMask = (1 << SlotPower) - 1;

		public Page[] Pages { get; private set; } = Array.Empty<Page>();

		public ushort PageCount { get; private set; }

		public Pointer[] NextToAlloc { get; } = new Pointer[AllClassCount];
		public Pointer[] FreeToAlloc { get; } = new Pointer[AllClassCount];

		public Allocator()
		{
			// Reserve default value of pointer to be invalid.
			Alloc(0, 0, MemoryInit.Uninitialized);
			Pages[0].UsedSlots[0] = 0; // Mark it not in use, but also not in free list.
		}

		~Allocator()
		{
			for (var i = 0; i < PageCount; i++)
			{
				UnsafeUtils.FreeAligned(Pages[i].AlignedPtr);
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public ref T Value<T>(Pointer pointer) where T : unmanaged
		{
			ref readonly var page = ref GetPage(pointer);

			AllocatorOutOfRangeException.ThrowIfNotFitsInSlot(Unmanaged<T>.SizeInBytes, page.SlotLength);

			return ref *(T*)(page.AlignedPtr + pointer.Offset);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public ref T ValueUnsafe<T>(Pointer pointer) where T : unmanaged
		{
			return ref *(T*)(Pages[pointer.Page].AlignedPtr + pointer.Offset);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public ref T GetAt<T>(Pointer pointer, int index) where T : unmanaged
		{
			ref readonly var page = ref GetPage(pointer);

			AllocatorOutOfRangeException.ThrowIfOutOfRangeExclusive((index * Unmanaged<T>.SizeInBytes) + Unmanaged<T>.SizeInBytes - 1, page.SlotLength);

			return ref ((T*)(page.AlignedPtr + pointer.Offset))[index];
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Pointer<T> Alloc<T>(int minimumLength, MemoryInit memoryInit = MemoryInit.Clear) where T : unmanaged
		{
			var info = Unmanaged<T>.Info;
			return (Pointer<T>)Alloc(minimumLength * info.Size, info.Alignment, memoryInit);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Pointer Alloc(int minimumLength, int alignment, MemoryInit memoryInit = MemoryInit.Clear)
		{
			NegativeArgumentException.ThrowIfNegative(minimumLength);
			NotPowerOfTwoArgumentException.ThrowIfNotPowerOfTwo(alignment);

			var slotClass = SlotClass(minimumLength, alignment);
			var slotClassIndex = slotClass - MinSlotClass;
			var slotLength = 1 << slotClass;

			var pointer = FreeToAlloc[slotClassIndex];
			if (pointer.IsNotNull)
			{
				ref var page = ref Pages[pointer.Page];
				FreeToAlloc[slotClassIndex] = *(Pointer*)(page.AlignedPtr + pointer.Offset);
				SetUsedSlot(page.UsedSlots, pointer.Offset >> slotClass);
				if (memoryInit == MemoryInit.Clear)
				{
					UnsafeUtils.Clear(page.AlignedPtr, pointer.Offset, slotLength);
				}

				return pointer;
			}

			pointer = NextToAlloc[slotClassIndex];
			if (pointer.Offset != 0)
			{
				ref var page = ref Pages[pointer.Page];
				NextToAlloc[slotClassIndex].Offset = unchecked((ushort)(pointer.Offset + slotLength));
				SetUsedSlot(page.UsedSlots, pointer.Offset >> slotClass);
				if (memoryInit == MemoryInit.Clear)
				{
					UnsafeUtils.Clear(page.AlignedPtr, pointer.Offset, slotLength);
				}
				return pointer;
			}

			EnsurePageAt(PageCount);
			var bitSetLength = BitSetLength(slotClass);
			var pageLength = PageLength(slotClass);

			Pages[PageCount] = new Page(UnsafeUtils.AllocAligned(pageLength + bitSetLength, MinPageLength), slotClass);
			NextToAlloc[slotClassIndex] = new Pointer() { Offset = (ushort)slotLength, Page = PageCount };

			UnsafeUtils.Clear((byte*)Pages[PageCount].UsedSlots, bitSetLength);
			SetUsedSlot(Pages[PageCount].UsedSlots, 0);

			if (memoryInit == MemoryInit.Clear)
			{
				UnsafeUtils.Clear(Pages[PageCount].AlignedPtr, slotLength);
			}

			return new Pointer() { Offset = 0, Page = PageCount++ };
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Resize<T>(ref Pointer pointer, int minimumLength, MemoryInit memoryInit = MemoryInit.Clear) where T : unmanaged
		{
			var info = Unmanaged<T>.Info;
			Resize(ref pointer, minimumLength * info.Size, info.Alignment, memoryInit);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Resize(ref Pointer pointer, int minimumLength, int alignment, MemoryInit memoryInit = MemoryInit.Clear)
		{
			NegativeArgumentException.ThrowIfNegative(minimumLength);
			NotPowerOfTwoArgumentException.ThrowIfNotPowerOfTwo(alignment);
			InvalidPointerException.ThrowIfNotAllocated(this, pointer);

			ref var page = ref Pages[pointer.Page];

			var newSizeClass = SlotClass(minimumLength, alignment);
			if (newSizeClass == page.SlotClass)
			{
				return;
			}

			var newPointer = Alloc(minimumLength, alignment, MemoryInit.Uninitialized);
			ref var newPage = ref Pages[newPointer.Page];

			var minSlotLength = 1 << MathUtils.Min(page.SlotClass, newPage.SlotClass);
			var destination = newPage.AlignedPtr + newPointer.Offset;
			UnsafeUtils.Copy(page.AlignedPtr + pointer.Offset, destination, minSlotLength);
			if (memoryInit == MemoryInit.Clear && newPage.SlotClass > page.SlotClass)
			{
				var start = newPointer.Offset + (1 << page.SlotClass);
				var remainingLength = 1 << (newPage.SlotClass - page.SlotClass);
				UnsafeUtils.Clear(newPage.AlignedPtr, start, remainingLength);
			}

			Free(pointer);

			pointer = newPointer;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool TryFree(Pointer pointer)
		{
			if (pointer.IsNull || pointer.Page >= PageCount)
			{
				return false;
			}

			ref var page = ref Pages[pointer.Page];

			var slot = pointer.Offset >> page.SlotClass;
			var bitsetIndex = slot >> SlotPower;
			var bitsetMask = SingleBit << (slot & SlotMask);

			if ((page.UsedSlots[bitsetIndex] & bitsetMask) != 0)
			{
				var slotClassIndex = page.SlotClass - MinSlotClass;
				*(Pointer*)(page.AlignedPtr + pointer.Offset) = FreeToAlloc[slotClassIndex];
				FreeToAlloc[slotClassIndex] = pointer;
				page.UsedSlots[bitsetIndex] &= ~bitsetMask;
				return true;
			}

			return false;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Free(Pointer pointer)
		{
			InvalidPointerException.ThrowIfNotAllocated(this, pointer);

			ref var page = ref Pages[pointer.Page];

			var slot = pointer.Offset >> page.SlotClass;
			var bitsetIndex = slot >> SlotPower;
			var bitsetMask = SingleBit << (slot & SlotMask);

			var slotClassIndex = page.SlotClass - MinSlotClass;
			*(Pointer*)(page.AlignedPtr + pointer.Offset) = FreeToAlloc[slotClassIndex];
			FreeToAlloc[slotClassIndex] = pointer;
			page.UsedSlots[bitsetIndex] &= ~bitsetMask;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool IsAllocated(Pointer pointer)
		{
			if (pointer.IsNull || pointer.Page >= PageCount)
			{
				return false;
			}

			ref var page = ref Pages[pointer.Page];

			var slot = pointer.Offset >> page.SlotClass;
			var bitsetIndex = slot >> SlotPower;
			var bitsetMask = SingleBit << (slot & SlotMask);

			return (page.UsedSlots[bitsetIndex] & bitsetMask) != 0;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public ref readonly Page GetPage(Pointer pointer)
		{
			InvalidPointerException.ThrowIfNotAllocated(this, pointer);

			return ref Pages[pointer.Page];
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public byte* GetPtr(Pointer pointer)
		{
			InvalidPointerException.ThrowIfNotAllocated(this, pointer);

			return Pages[pointer.Page].AlignedPtr + pointer.Offset;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void EnsurePageAt(int index)
		{
			if (index >= Pages.Length)
			{
				Pages = Pages.ResizeToNextPowOf2(index + 1);
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int SlotClass(int length, int alignment)
		{
			return MathUtils.CeilLog2((uint)MathUtils.Max(length, alignment, MinSlotLength));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int BitSetLength(int slotClass)
		{
			var slotCount = 1 << MathUtils.Max(MinPageSlotClass - slotClass, SlotPower);
			return (slotCount + 7) >> 3;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int PageLength(int slotClass)
		{
			return 1 << MathUtils.Max(slotClass, MinPageSlotClass);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void SetUsedSlot(uint* usedBits, int slot)
		{
			usedBits[slot >> SlotPower] |= SingleBit << (slot & SlotMask);
		}

		public void SetPageCount(ushort pageCount)
		{
			PageCount = pageCount;
		}

		public void Reset()
		{
			for (var i = 1; i < PageCount; i++)
			{
				UnsafeUtils.FreeAligned(Pages[i].AlignedPtr);
				Pages[i] = default;
			}
			PageCount = 1;

			UnsafeUtils.Clear((byte*)Pages[0].UsedSlots, BitSetLength(MinSlotClass));

			Array.Clear(NextToAlloc, 0, AllClassCount);
			Array.Clear(FreeToAlloc, 0, AllClassCount);

			NextToAlloc[0].Offset = MinSlotLength;
		}

		public Allocator Clone()
		{
			var clone = new Allocator();
			CopyTo(clone);
			return clone;
		}

		public void CopyTo(Allocator other)
		{
			other.EnsurePageAt(PageCount - 1);

			var encounteredSlotClasses = stackalloc bool[AllClassCount];
			for (var i = 0; i < AllClassCount; i++)
			{
				encounteredSlotClasses[i] = false;
			}

			// Leverage the fact that we know used length of last pages of each slot class.
			for (var i = PageCount - 1; i >= 0; i--)
			{
				ref var page = ref Pages[i];
				ref var otherPage = ref other.Pages[i];

				var pageSlotClass = page.SlotClass;
				var bitsetLength = BitSetLength(page.SlotClass);
				var fullPageLength = PageLength(page.SlotClass) + bitsetLength;

				if (pageSlotClass != otherPage.SlotClass)
				{
					UnsafeUtils.FreeAligned(otherPage.AlignedPtr);
					otherPage = new Page(UnsafeUtils.AllocAligned(fullPageLength, MinPageLength), pageSlotClass);
				}

				var isLastPageOfSlotClass = !encounteredSlotClasses[pageSlotClass] && NextToAlloc[pageSlotClass].Offset > 0;
				if (isLastPageOfSlotClass)
				{
					UnsafeUtils.Copy(page.AlignedPtr, otherPage.AlignedPtr, NextToAlloc[pageSlotClass].Offset);
					UnsafeUtils.Copy(page.UsedSlots, otherPage.UsedSlots, bitsetLength);
					encounteredSlotClasses[pageSlotClass] = true;
				}
				else
				{
					UnsafeUtils.Copy(page.AlignedPtr, otherPage.AlignedPtr, fullPageLength);
				}
			}

			for (var i = PageCount; i < other.PageCount; i++)
			{
				UnsafeUtils.FreeAligned(other.Pages[i].AlignedPtr);
			}

			Array.Copy(FreeToAlloc, other.FreeToAlloc, AllClassCount);
			Array.Copy(NextToAlloc, other.NextToAlloc, AllClassCount);

			other.PageCount = PageCount;
		}
	}
}
