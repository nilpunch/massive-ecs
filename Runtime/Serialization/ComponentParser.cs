using System;
using System.IO;

// ReSharper disable MustUseReturnValue
namespace Massive.Serialization
{
	public class ComponentParser<T> : IRegistryParser where T : unmanaged
	{
		public unsafe void Write(IRegistry registry, Stream stream)
		{
			var set = registry.Any<T>();

			SparseSetParser.Write(set, stream);

			if (set is IDataSet<T> dataSet)
			{
				fixed (T* data = dataSet.RawData)
				{
					stream.Write(new ReadOnlySpan<byte>(data, set.Count * sizeof(T)));
				}
			}
		}

		public unsafe void Read(IRegistry registry, Stream stream)
		{
			var set = registry.Any<T>();

			SparseSetParser.Read(set, stream);

			if (set is IDataSet<T> dataSet)
			{
				fixed (T* data = dataSet.RawData)
				{
					stream.Read(new Span<byte>(data, set.Count * sizeof(T)));
				}
			}
		}
	}
}