using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

// ReSharper disable MustUseReturnValue
namespace Massive.Serialization
{
	public static class SerializationHelpers
	{
		private static readonly byte[] s_buffer4Bytes = new byte[4];

		public static void WriteSparseSet(SparseSet set, Stream stream)
		{
			WriteInt(set.Count, stream);
			WriteInt(set.SparseCapacity, stream);

			stream.Write(MemoryMarshal.Cast<int, byte>(set.Packed.AsSpan(0, set.Count)));
			stream.Write(MemoryMarshal.Cast<int, byte>(set.Sparse.AsSpan(0, set.SparseCapacity)));
		}

		public static void ReadSparseSet(SparseSet set, Stream stream)
		{
			set.Count = ReadInt(stream);

			var sparseCapacity = ReadInt(stream);

			set.ResizePacked(set.Count);
			set.ResizeSparse(sparseCapacity);

			stream.Read(MemoryMarshal.Cast<int, byte>(set.Packed.AsSpan(0, set.Count)));
			stream.Read(MemoryMarshal.Cast<int, byte>(set.Sparse.AsSpan(0, set.SparseCapacity)));
		}

		public static unsafe void WriteUnmanagedPagedArray(IPagedArray pagedArray, int count, Stream stream)
		{
			var underlyingType = pagedArray.DataType.IsEnum ? Enum.GetUnderlyingType(pagedArray.DataType) : pagedArray.DataType;
			var sizeOfItem = Marshal.SizeOf(underlyingType);

			foreach (var (pageIndex, pageLength, _) in new PageSequence(pagedArray.PageSize, count))
			{
				var page = pagedArray.GetPage(pageIndex);
				var handle = GCHandle.Alloc(page, GCHandleType.Pinned);
				var pageAsSpan = new Span<byte>(handle.AddrOfPinnedObject().ToPointer(), pageLength * sizeOfItem);
				stream.Write(pageAsSpan);
				handle.Free();
			}
		}

		public static unsafe void ReadUnmanagedPagedArray(IPagedArray pagedArray, int count, Stream stream)
		{
			var underlyingType = pagedArray.DataType.IsEnum ? Enum.GetUnderlyingType(pagedArray.DataType) : pagedArray.DataType;
			var sizeOfItem = Marshal.SizeOf(underlyingType);

			foreach (var (pageIndex, pageLength, _) in new PageSequence(pagedArray.PageSize, count))
			{
				pagedArray.EnsurePage(pageIndex);

				var page = pagedArray.GetPage(pageIndex);
				var handle = GCHandle.Alloc(page, GCHandleType.Pinned);
				var pageAsSpan = new Span<byte>(handle.AddrOfPinnedObject().ToPointer(), pageLength * sizeOfItem);
				stream.Read(pageAsSpan);
				handle.Free();
			}
		}

		public static void WriteManagedPagedArray(IPagedArray pagedArray, int count, Stream stream)
		{
			var binaryFormatter = new BinaryFormatter();
			var buffer = Array.CreateInstance(pagedArray.DataType, count);

			foreach (var (pageIndex, pageLength, indexOffset) in new PageSequence(pagedArray.PageSize, count))
			{
				Array.Copy(pagedArray.GetPage(pageIndex), 0, buffer, indexOffset, pageLength);
			}

			binaryFormatter.Serialize(stream, buffer);
		}

		public static void ReadManagedPagedArray(IPagedArray pagedArray, int count, Stream stream)
		{
			var binaryFormatter = new BinaryFormatter();
			var buffer = (Array)binaryFormatter.Deserialize(stream);

			foreach (var (pageIndex, pageLength, indexOffset) in new PageSequence(pagedArray.PageSize, count))
			{
				pagedArray.EnsurePage(pageIndex);
				Array.Copy(buffer, indexOffset, pagedArray.GetPage(pageIndex), 0, pageLength);
			}
		}

		public static void WriteInt(int value, Stream stream)
		{
			BitConverter.TryWriteBytes(s_buffer4Bytes, value);
			stream.Write(s_buffer4Bytes);
		}

		public static int ReadInt(Stream stream)
		{
			stream.Read(s_buffer4Bytes);
			return BitConverter.ToInt32(s_buffer4Bytes);
		}

		public static void WriteBool(bool value, Stream stream)
		{
			BitConverter.TryWriteBytes(s_buffer4Bytes, value);
			stream.Write(s_buffer4Bytes, 0, 1);
		}

		public static bool ReadBool(Stream stream)
		{
			stream.Read(s_buffer4Bytes);
			return BitConverter.ToBoolean(s_buffer4Bytes);
		}

		public static void WriteType(Type type, Stream stream)
		{
			var typeName = type.AssemblyQualifiedName!;
			var nameBuffer = Encoding.UTF8.GetBytes(typeName);

			WriteInt(nameBuffer.Length, stream);
			stream.Write(nameBuffer);
		}

		public static Type ReadType(Stream stream)
		{
			var nameLength = ReadInt(stream);
			var nameBuffer = new byte[nameLength];

			stream.Read(nameBuffer);

			var typeName = Encoding.UTF8.GetString(nameBuffer);
			return Type.GetType(typeName, true);
		}
	}
}
