using System;
using System.IO;
using System.Runtime.InteropServices;

// ReSharper disable MustUseReturnValue
namespace Massive.Serialization
{
	public class EntitiesSerializer : IRegistrySerializer
	{
		private static readonly byte[] s_buffer4Bytes = new byte[4];

		public void Serialize(Registry registry, Stream stream)
		{
			var entities = registry.Entities;

			BitConverter.TryWriteBytes(s_buffer4Bytes, entities.Count);
			stream.Write(s_buffer4Bytes);

			BitConverter.TryWriteBytes(s_buffer4Bytes, entities.MaxId);
			stream.Write(s_buffer4Bytes);

			stream.Write(MemoryMarshal.Cast<Entity, byte>(entities.Dense.AsSpan(0, entities.MaxId)));
			stream.Write(MemoryMarshal.Cast<int, byte>(entities.Sparse.AsSpan(0, entities.MaxId)));
		}

		public void Deserialize(Registry registry, Stream stream)
		{
			var entities = registry.Entities;

			stream.Read(s_buffer4Bytes);
			entities.Count = BitConverter.ToInt32(s_buffer4Bytes);

			stream.Read(s_buffer4Bytes);
			entities.MaxId = BitConverter.ToInt32(s_buffer4Bytes);

			entities.ResizeDense(entities.MaxId);
			entities.ResizeSparse(entities.MaxId);

			stream.Read(MemoryMarshal.Cast<Entity, byte>(entities.Dense.AsSpan(0, entities.MaxId)));
			stream.Read(MemoryMarshal.Cast<int, byte>(entities.Sparse.AsSpan(0, entities.MaxId)));
		}
	}
}
