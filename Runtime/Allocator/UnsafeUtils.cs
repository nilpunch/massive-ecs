using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Massive
{
	public static class UnsafeUtils
	{
		public static unsafe byte* AllocAligned(int size, int alignment)
		{
			var originalPtr = Marshal.AllocHGlobal((IntPtr)(size + alignment - 1 + IntPtr.Size));

			var alignedPtr = (byte*)((originalPtr.ToInt64() + IntPtr.Size + (alignment - 1)) & ~(alignment - 1));

			((IntPtr*)alignedPtr)[-1] = originalPtr;

			return alignedPtr;
		}

		public static unsafe void FreeAligned(byte* alignedPtr)
		{
			if ((ulong)alignedPtr == 0)
			{
				return;
			}

			var originalPtr = ((IntPtr*)alignedPtr)[-1];
			Marshal.FreeHGlobal(originalPtr);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static unsafe void Copy(void* source, void* destination, int length)
		{
			Copy((byte*)source, (byte*)destination, length);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static unsafe void Copy(byte* source, byte* destination, int length)
		{
			if (length <= 0)
			{
				return;
			}

			if (length > 128)
			{
				Buffer.MemoryCopy(source, destination, length, length);
				return;
			}

			var ptr = source;
			var end = ptr + length;

			while (((ulong)source & 7) != 0 && source < end)
			{
				*destination++ = *source++;
			}

			var sourceLong = (long*)source;
			var destinationLong = (long*)destination;
			while ((byte*)(sourceLong + 1) <= end)
			{
				*destinationLong++ = *sourceLong++;
			}

			source = (byte*)sourceLong;
			while (source < end)
			{
				*destination++ = *source++;
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static unsafe void Clear(byte* data, int length)
		{
			Clear(data, 0, length);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static unsafe void Clear(byte* data, int start, int length)
		{
			if (length <= 0)
				return;

			var ptr = data + start;
			var end = ptr + length;

			while (((ulong)ptr & 7) != 0 && ptr < end)
			{
				*ptr++ = 0;
			}

			var ptrLong = (long*)ptr;
			while ((byte*)(ptrLong + 1) <= end)
			{
				*ptrLong++ = 0L;
			}

			ptr = (byte*)ptrLong;
			while (ptr < end)
			{
				*ptr++ = 0;
			}
		}
	}
}
