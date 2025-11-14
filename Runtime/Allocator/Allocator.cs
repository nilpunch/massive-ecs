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
	public unsafe partial class Allocator
	{
		public readonly struct Page
		{
			public readonly byte* AlignedPtr;

			public readonly uint* UsedSlots;

			public readonly int SizeClass;

			public Page(byte* alignedPtr, int sizeClass)
			{
				AlignedPtr = alignedPtr;
				SizeClass = sizeClass;

				var pageSize = 1 << MathUtils.Max(sizeClass, MinPageSizeClass);
				UsedSlots = (uint*)(alignedPtr + pageSize);
			}
		}

		public const int MinPageSizeClass = 16;
		public const int MinPageSize = 1 << MinPageSizeClass;
		public const int MinSmallClass = 2; // Alignment for uint bitset at the end and free list of Pointer's.
		public const int MinSmallClassSize = 1 << MinSmallClass;
		public const int MaxSmallClass = MinPageSizeClass;
		public const int AllClassCount = 32 - MinSmallClass;

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
		public ref T Get<T>(Pointer pointer) where T : unmanaged
		{
			return ref *(T*)(GetPage(pointer).AlignedPtr + pointer.Offset);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public ref T GetAt<T>(Pointer pointer, int index) where T : unmanaged
		{
			ref readonly var page = ref GetPage(pointer);

			AllocatorIndexOutOfRangeException.ThrowIfOutOfRangeExclusive(index * Unmanaged<T>.SizeInBytes, 1 << page.SizeClass);

			return ref ((T*)(page.AlignedPtr + pointer.Offset))[index];
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Pointer Alloc<T>(int minimumLength, MemoryInit memoryInit = MemoryInit.Clear) where T : unmanaged
		{
			var info = Unmanaged<T>.Info;
			return Alloc(minimumLength * info.Size, info.Alignment, memoryInit);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Pointer Alloc(int minimumLength, int alignment, MemoryInit memoryInit = MemoryInit.Clear)
		{
			NotPowerOfTwoArgumentException.ThrowIfNotPowerOfTwo(alignment);

			var sizeClass = SizeClass(minimumLength, alignment);
			var sizeClassIndex = sizeClass - MinSmallClass;
			var slotSize = 1 << sizeClass;

			var pointer = FreeToAlloc[sizeClassIndex];
			if (pointer.IsNotNull)
			{
				ref var page = ref Pages[pointer.Page];
				FreeToAlloc[sizeClassIndex] = *(Pointer*)(page.AlignedPtr + pointer.Offset);
				SetUsedSlot(page.UsedSlots, pointer.Offset >> sizeClass);
				if (memoryInit == MemoryInit.Clear)
				{
					UnsafeUtils.Clear(page.AlignedPtr, pointer.Offset, slotSize);
				}

				return pointer;
			}

			pointer = NextToAlloc[sizeClassIndex];
			if (pointer.Offset != 0)
			{
				ref var page = ref Pages[pointer.Page];
				NextToAlloc[sizeClassIndex].Offset = unchecked((ushort)(pointer.Offset + slotSize));
				SetUsedSlot(page.UsedSlots, pointer.Offset >> sizeClass);
				if (memoryInit == MemoryInit.Clear)
				{
					UnsafeUtils.Clear(page.AlignedPtr, pointer.Offset, slotSize);
				}
				return pointer;
			}

			EnsurePageAt(PageCount);
			var bitSetLength = BitSetLength(sizeClass);
			var pageSize = 1 << MathUtils.Max(sizeClass, MinPageSizeClass);

			Pages[PageCount] = new Page(UnsafeUtils.AllocAligned(pageSize + bitSetLength, MinPageSize), sizeClass);
			NextToAlloc[sizeClassIndex] = new Pointer() { Offset = (ushort)slotSize, Page = PageCount };

			UnsafeUtils.Clear((byte*)Pages[PageCount].UsedSlots, bitSetLength);
			SetUsedSlot(Pages[PageCount].UsedSlots, 0);

			if (memoryInit == MemoryInit.Clear)
			{
				UnsafeUtils.Clear(Pages[PageCount].AlignedPtr, slotSize);
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
			NotPowerOfTwoArgumentException.ThrowIfNotPowerOfTwo(alignment);
			InvalidPointerException.ThrowIfNotAllocated(this, pointer);

			ref var page = ref Pages[pointer.Page];

			var newSizeClass = SizeClass(minimumLength, alignment);
			if (newSizeClass == page.SizeClass)
			{
				return;
			}

			var newPointer = Alloc(minimumLength, alignment, MemoryInit.Uninitialized);
			ref var newPage = ref Pages[newPointer.Page];

			var minSlotSize = 1 << MathUtils.Min(page.SizeClass, newPage.SizeClass);
			var destination = newPage.AlignedPtr + newPointer.Offset;
			UnsafeUtils.Copy(page.AlignedPtr + pointer.Offset, destination, minSlotSize);
			if (memoryInit == MemoryInit.Clear && newPage.SizeClass > page.SizeClass)
			{
				var start = newPointer.Offset + (1 << page.SizeClass);
				var remainingLength = 1 << (newPage.SizeClass - page.SizeClass);
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

			var slot = pointer.Offset >> page.SizeClass;
			var bitsetIndex = slot >> 5;
			var bitsetMask = 1U << (slot & 31);

			if ((page.UsedSlots[bitsetIndex] & bitsetMask) != 0)
			{
				var sizeClassIndex = page.SizeClass - MinSmallClass;
				*(Pointer*)(page.AlignedPtr + pointer.Offset) = FreeToAlloc[sizeClassIndex];
				FreeToAlloc[sizeClassIndex] = pointer;
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

			var slot = pointer.Offset >> page.SizeClass;
			var bitsetIndex = slot >> 5;
			var bitsetMask = 1U << (slot & 31);

			var sizeClassIndex = page.SizeClass - MinSmallClass;
			*(Pointer*)(page.AlignedPtr + pointer.Offset) = FreeToAlloc[sizeClassIndex];
			FreeToAlloc[sizeClassIndex] = pointer;
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

			var slot = pointer.Offset >> page.SizeClass;
			var bitsetIndex = slot >> 5;
			var bitsetMask = 1U << (slot & 31);

			return (page.UsedSlots[bitsetIndex] & bitsetMask) != 0;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public ref readonly Page GetPage(Pointer pointer)
		{
			InvalidPointerException.ThrowIfNotAllocated(this, pointer);

			return ref Pages[pointer.Page];
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
		private int SizeClass(int length, int alignment)
		{
			return MathUtils.CeilLog2(MathUtils.Max(length, alignment, MinSmallClassSize));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private int BitSetLength(int sizeClass)
		{
			var slotCount = 1 << MathUtils.Max(MinPageSizeClass - sizeClass, 0);
			return (slotCount + 7) >> 3;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void SetUsedSlot(uint* usedBits, int slot)
		{
			usedBits[slot >> 5] |= 1U << (slot & 31);
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

			for (var i = 0; i < PageCount; i++)
			{
				ref var page = ref Pages[i];
				ref var otherPage = ref other.Pages[i];

				var pageSizeClass = page.SizeClass;
				var bitsetLength = BitSetLength(page.SizeClass);
				var pageSize = (1 << MathUtils.Max(pageSizeClass, MinPageSizeClass)) + bitsetLength;

				if (pageSizeClass != otherPage.SizeClass)
				{
					UnsafeUtils.FreeAligned(otherPage.AlignedPtr);
					otherPage = new Page(UnsafeUtils.AllocAligned(pageSize, MinPageSize), pageSizeClass);
				}

				Buffer.MemoryCopy(page.AlignedPtr, otherPage.AlignedPtr, pageSize, pageSize);
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
