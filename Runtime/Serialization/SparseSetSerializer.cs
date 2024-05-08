using System;
using System.IO;

// ReSharper disable MustUseReturnValue
namespace Massive.Serialization
{
	public static class SparseSetSerializer
	{
		private static readonly byte[] s_buffer4Bytes = new byte[4];

		public static unsafe void Serialize(SparseSet set, Stream stream)
		{
			BitConverter.TryWriteBytes(s_buffer4Bytes, set.Count);
			stream.Write(s_buffer4Bytes);

			BitConverter.TryWriteBytes(s_buffer4Bytes, set.SparseCapacity);
			stream.Write(s_buffer4Bytes);

			fixed (int* dense = set.Dense)
			{
				stream.Write(new ReadOnlySpan<byte>(dense, set.Count * sizeof(int)));
			}

			fixed (int* sparse = set.Sparse)
			{
				stream.Write(new ReadOnlySpan<byte>(sparse, set.SparseCapacity * sizeof(int)));
			}
		}

		public static unsafe void Deserialize(SparseSet set, Stream stream)
		{
			stream.Read(s_buffer4Bytes);
			set.Count = BitConverter.ToInt32(s_buffer4Bytes);
			set.ResizeDense(set.Count);

			stream.Read(s_buffer4Bytes);
			var sparseCapacity = BitConverter.ToInt32(s_buffer4Bytes);
			set.ResizeSparse(sparseCapacity);

			fixed (int* dense = set.Dense)
			{
				stream.Read(new Span<byte>(dense, set.Count * sizeof(int)));
			}

			fixed (int* sparse = set.Sparse)
			{
				stream.Read(new Span<byte>(sparse, set.SparseCapacity * sizeof(int)));
			}
		}
	}
}
