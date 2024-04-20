using System;
using System.IO;
using System.Runtime.InteropServices;

// ReSharper disable MustUseReturnValue
namespace Massive.Serialization
{
	public class ComponentParser<T> : IRegistryParser where T : unmanaged
	{
		private readonly byte[] _buffer4Bytes = new byte[4];

		public unsafe void Write(IRegistry registry, Stream stream)
		{
			var set = registry.Any<T>();

			SparseSetParser.Write(set, stream);

			if (set is IDataSet<T> dataSet)
			{
				fixed (T* data = dataSet.Data)
				{
					stream.Write(new ReadOnlySpan<byte>(data, set.AliveCount * Marshal.SizeOf<T>()));
				}
			}
		}

		public unsafe void Read(IRegistry registry, Stream stream)
		{
			var set = registry.Any<T>();

			SparseSetParser.Read(set, stream);

			if (set is IDataSet<T> dataSet)
			{
				fixed (T* data = dataSet.Data)
				{
					stream.Read(new Span<byte>(data, set.AliveCount * sizeof(T)));
				}
			}
		}
	}
}