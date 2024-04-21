using System;
using System.IO;

// ReSharper disable MustUseReturnValue
namespace Massive.Serialization
{
	public static class SparseSetParser
	{
		private static readonly byte[] _buffer4Bytes = new byte[4];

		public unsafe static void Write(ISet set, Stream stream)
		{
			BitConverter.TryWriteBytes(_buffer4Bytes, set.Count);
			stream.Write(_buffer4Bytes);

			BitConverter.TryWriteBytes(_buffer4Bytes, set.SparseCapacity);
			stream.Write(_buffer4Bytes);

			fixed (int* dense = set.Dense)
			{
				stream.Write(new ReadOnlySpan<byte>(dense, set.Count * sizeof(int)));
			}

			fixed (int* sparse = set.Sparse)
			{
				stream.Write(new ReadOnlySpan<byte>(sparse, set.SparseCapacity * sizeof(int)));
			}
		}

		public unsafe static void Read(ISet set, Stream stream)
		{
			stream.Read(_buffer4Bytes);
			set.Count = BitConverter.ToInt32(_buffer4Bytes);
			set.ResizeDense(set.Count);

			stream.Read(_buffer4Bytes);
			var sparseCapacity = BitConverter.ToInt32(_buffer4Bytes);
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