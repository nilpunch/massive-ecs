using System;
using System.IO;
using System.Runtime.InteropServices;

// ReSharper disable MustUseReturnValue
namespace Massive.Serialization
{
	public static class SparseSetSerializer
	{
		private static readonly byte[] s_buffer4Bytes = new byte[4];

		public static void Serialize(SparseSet set, Stream stream)
		{
			BitConverter.TryWriteBytes(s_buffer4Bytes, set.Count);
			stream.Write(s_buffer4Bytes);

			BitConverter.TryWriteBytes(s_buffer4Bytes, set.SparseCapacity);
			stream.Write(s_buffer4Bytes);

			if (!set.InPlace)
			{
				stream.Write(MemoryMarshal.Cast<int, byte>(set.Dense.AsSpan(0, set.Count)));
			}
			stream.Write(MemoryMarshal.Cast<int, byte>(set.Sparse.AsSpan(0, set.SparseCapacity)));
		}

		public static void Deserialize(SparseSet set, Stream stream)
		{
			stream.Read(s_buffer4Bytes);
			set.Count = BitConverter.ToInt32(s_buffer4Bytes);
			set.ResizeDense(set.Count);

			stream.Read(s_buffer4Bytes);
			var sparseCapacity = BitConverter.ToInt32(s_buffer4Bytes);
			set.ResizeSparse(sparseCapacity);

			if (!set.InPlace)
			{
				stream.Read(MemoryMarshal.Cast<int, byte>(set.Dense.AsSpan(0, set.Count)));
			}
			stream.Read(MemoryMarshal.Cast<int, byte>(set.Sparse.AsSpan(0, set.SparseCapacity)));
		}
	}
}
