using System;
using System.IO;

// ReSharper disable MustUseReturnValue
namespace Massive.Serialization
{
	public class ComponentSerializer<T> : IRegistrySerializer where T : unmanaged
	{
		public unsafe void Serialize(IRegistry registry, Stream stream)
		{
			var set = (SparseSet)registry.Any<T>();

			SparseSetSerializer.Serialize(set, stream);

			// if (set is DataSet<T> dataSet)
			// {
			// 	fixed (T* data = dataSet.RawData)
			// 	{
			// 		stream.Write(new ReadOnlySpan<byte>(data, set.Count * sizeof(T)));
			// 	}
			// }
		}

		public unsafe void Deserialize(IRegistry registry, Stream stream)
		{
			var set = (SparseSet)registry.Any<T>();

			SparseSetSerializer.Deserialize(set, stream);

			// if (set is DataSet<T> dataSet)
			// {
			// 	fixed (T* data = dataSet.RawData)
			// 	{
			// 		stream.Read(new Span<byte>(data, set.Count * sizeof(T)));
			// 	}
			// }
		}
	}
}
