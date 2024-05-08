using System;
using System.IO;

// ReSharper disable MustUseReturnValue
namespace Massive.Serialization
{
	public class EntitiesSerializer : IRegistrySerializer
	{
		private static readonly byte[] s_buffer4Bytes = new byte[4];

		public unsafe void Serialize(IRegistry registry, Stream stream)
		{
			var entities = (Entities)registry.Entities;

			BitConverter.TryWriteBytes(s_buffer4Bytes, entities.Count);
			stream.Write(s_buffer4Bytes);

			BitConverter.TryWriteBytes(s_buffer4Bytes, entities.MaxId);
			stream.Write(s_buffer4Bytes);

			fixed (Entity* dense = entities.Dense)
			{
				stream.Write(new ReadOnlySpan<byte>(dense, entities.MaxId * sizeof(Entity)));
			}

			fixed (int* sparse = entities.Sparse)
			{
				stream.Write(new ReadOnlySpan<byte>(sparse, entities.MaxId * sizeof(int)));
			}
		}

		public unsafe void Deserialize(IRegistry registry, Stream stream)
		{
			var entities = (Entities)registry.Entities;

			stream.Read(s_buffer4Bytes);
			entities.Count = BitConverter.ToInt32(s_buffer4Bytes);

			stream.Read(s_buffer4Bytes);
			entities.MaxId = BitConverter.ToInt32(s_buffer4Bytes);

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
