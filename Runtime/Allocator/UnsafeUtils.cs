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
			AllocatorAlignmentsException.ThrowIfNotEqualAlignemnt(source, destination);

			if (length <= 8)
			{
				for (var i = 0; i < length; i++)
				{
					destination[i] = source[i];
				}
				return;
			}

			if (length > 128)
			{
				Buffer.MemoryCopy(source, destination, length, length);
				return;
			}

			var end = source + length;

			var misalignment = (int)((ulong)source & 7);
			if (misalignment != 0)
			{
				var bytesToAlign = 8 - misalignment;

				for (var i = 0; i < bytesToAlign; i++)
				{
					destination[i] = source[i];
				}

				source += bytesToAlign;
				destination += bytesToAlign;
			}

			var sourceLong = (long*)source;
			var destinationLong = (long*)destination;
			var longCount = (int)((end - source) >> 3);

			for (var i = 0; i < longCount; i++)
			{
				destinationLong[i] = sourceLong[i];
			}

			var bulkCopiedCount = longCount << 3;
			source += bulkCopiedCount;
			destination += bulkCopiedCount;
			var remaining = (int)(end - source);

			for (var i = 0; i < remaining; i++)
			{
				destination[i] = source[i];
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
			data += start;

			if (length <= 8)
			{
				for (var i = 0; i < length; i++)
				{
					data[i] = data[i];
				}
				return;
			}

			var end = data + length;

			var misalignment = (int)((ulong)data & 7);
			if (misalignment != 0)
			{
				var bytesToAlign = 8 - misalignment;

				for (var i = 0; i < bytesToAlign; i++)
				{
					data[i] = 0;
				}

				data += bytesToAlign;
			}

			var dataLong = (long*)data;
			var longCount = (int)((end - data) >> 3);

			for (var i = 0; i < longCount; i++)
			{
				dataLong[i] = 0;
			}

			var bulkCopiedCount = longCount << 3;
			data += bulkCopiedCount;
			var remaining = (int)(end - data);

			for (var i = 0; i < remaining; i++)
			{
				data[i] = 0;
			}
		}
	}
}
