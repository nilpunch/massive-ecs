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

		public static void WriteEntities(Entities entities, Stream stream)
		{
			WriteInt(entities.Count, stream);
			WriteInt(entities.MaxId, stream);

			stream.Write(MemoryMarshal.Cast<int, byte>(entities.Ids.AsSpan(0, entities.MaxId)));
			stream.Write(MemoryMarshal.Cast<uint, byte>(entities.Reuses.AsSpan(0, entities.MaxId)));
			stream.Write(MemoryMarshal.Cast<int, byte>(entities.Sparse.AsSpan(0, entities.MaxId)));
		}

		public static void ReadEntities(Entities entities, Stream stream)
		{
			entities.Count = ReadInt(stream);
			entities.MaxId = ReadInt(stream);

			entities.EnsureCapacityForIndex(entities.MaxId);

			stream.Read(MemoryMarshal.Cast<int, byte>(entities.Ids.AsSpan(0, entities.MaxId)));
			stream.Read(MemoryMarshal.Cast<uint, byte>(entities.Reuses.AsSpan(0, entities.MaxId)));
			stream.Read(MemoryMarshal.Cast<int, byte>(entities.Sparse.AsSpan(0, entities.MaxId)));
		}

		public static void WriteSparseSet(SparseSet set, Stream stream)
		{
			WriteInt(set.Count, stream);
			WriteInt(set.SparseCapacity, stream);
			WriteInt(set.NextHole, stream);

			stream.Write(MemoryMarshal.Cast<int, byte>(set.Packed.AsSpan(0, set.Count)));
			stream.Write(MemoryMarshal.Cast<int, byte>(set.Sparse.AsSpan(0, set.SparseCapacity)));
		}

		public static void ReadSparseSet(SparseSet set, Stream stream)
		{
			set.Count = ReadInt(stream);
			var sparseCount = ReadInt(stream);
			set.NextHole = ReadInt(stream);

			set.EnsurePackedForIndex(set.Count - 1);
			set.EnsureSparseForIndex(sparseCount - 1);

			stream.Read(MemoryMarshal.Cast<int, byte>(set.Packed.AsSpan(0, set.Count)));
			stream.Read(MemoryMarshal.Cast<int, byte>(set.Sparse.AsSpan(0, sparseCount)));
			if (sparseCount < set.SparseCapacity)
			{
				Array.Fill(set.Sparse, Constants.InvalidId, sparseCount, set.SparseCapacity - sparseCount);
			}
		}

		public static unsafe void WriteUnmanagedPagedArray(IPagedArray pagedArray, int count, Stream stream)
		{
			var underlyingType = pagedArray.ElementType.IsEnum ? Enum.GetUnderlyingType(pagedArray.ElementType) : pagedArray.ElementType;
			var sizeOfItem = Marshal.SizeOf(underlyingType);

			foreach (var (pageIndex, pageLength, _) in new PageSequence(pagedArray.PageSize, count))
			{
				var handle = GCHandle.Alloc(pagedArray.GetPage(pageIndex), GCHandleType.Pinned);
				var pageAsSpan = new Span<byte>(handle.AddrOfPinnedObject().ToPointer(), pageLength * sizeOfItem);
				stream.Write(pageAsSpan);
				handle.Free();
			}
		}

		public static unsafe void ReadUnmanagedPagedArray(IPagedArray pagedArray, int count, Stream stream)
		{
			var underlyingType = pagedArray.ElementType.IsEnum ? Enum.GetUnderlyingType(pagedArray.ElementType) : pagedArray.ElementType;
			var sizeOfItem = Marshal.SizeOf(underlyingType);

			foreach (var (pageIndex, pageLength, _) in new PageSequence(pagedArray.PageSize, count))
			{
				pagedArray.EnsurePage(pageIndex);

				var handle = GCHandle.Alloc(pagedArray.GetPage(pageIndex), GCHandleType.Pinned);
				var pageAsSpan = new Span<byte>(handle.AddrOfPinnedObject().ToPointer(), pageLength * sizeOfItem);
				stream.Read(pageAsSpan);
				handle.Free();
			}
		}

		public static void WriteManagedPagedArray(IPagedArray pagedArray, int count, Stream stream)
		{
			var binaryFormatter = new BinaryFormatter();
			var buffer = Array.CreateInstance(pagedArray.ElementType, count);

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
