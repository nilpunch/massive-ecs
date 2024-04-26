using System;
using System.IO;

// ReSharper disable MustUseReturnValue
namespace Massive.Serialization
{
	public class EntitiesParser : IRegistryParser
	{
		private static readonly byte[] _buffer4Bytes = new byte[4];

		public unsafe void Write(IRegistry registry, Stream stream)
		{
			var entities = registry.Entities;

			BitConverter.TryWriteBytes(_buffer4Bytes, entities.Count);
			stream.Write(_buffer4Bytes);

			BitConverter.TryWriteBytes(_buffer4Bytes, entities.MaxId);
			stream.Write(_buffer4Bytes);

			fixed (Entity* dense = entities.Dense)
			{
				stream.Write(new ReadOnlySpan<byte>(dense, entities.MaxId * sizeof(Entity)));
			}

			fixed (int* sparse = entities.Sparse)
			{
				stream.Write(new ReadOnlySpan<byte>(sparse, entities.MaxId * sizeof(int)));
			}
		}

		public unsafe void Read(IRegistry registry, Stream stream)
		{
			var entities = registry.Entities;

			stream.Read(_buffer4Bytes);
			entities.Count = BitConverter.ToInt32(_buffer4Bytes);

			stream.Read(_buffer4Bytes);
			entities.MaxId = BitConverter.ToInt32(_buffer4Bytes);

			entities.ResizeDense(entities.MaxId);
			entities.ResizeSparse(entities.MaxId);

			fixed (Entity* dense = entities.Dense)
			{
				stream.Read(new Span<byte>(dense, entities.MaxId * sizeof(Entity)));
			}

			fixed (int* sparse = entities.Sparse)
			{
				stream.Read(new Span<byte>(sparse, entities.MaxId * sizeof(int)));
			}
		}
	}
}
