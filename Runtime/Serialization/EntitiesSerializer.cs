using System;
using System.IO;
using System.Runtime.InteropServices;

// ReSharper disable MustUseReturnValue
namespace Massive.Serialization
{
	public class EntitiesSerializer : IRegistrySerializer
	{
		public void Serialize(Registry registry, Stream stream)
		{
			var entities = registry.Entities;

			SerializationHelpers.WriteInt(entities.Count, stream);
			SerializationHelpers.WriteInt(entities.MaxId, stream);

			stream.Write(MemoryMarshal.Cast<int, byte>(entities.Ids.AsSpan(0, entities.MaxId)));
			stream.Write(MemoryMarshal.Cast<uint, byte>(entities.Reuses.AsSpan(0, entities.MaxId)));
			stream.Write(MemoryMarshal.Cast<int, byte>(entities.Sparse.AsSpan(0, entities.MaxId)));
		}

		public void Deserialize(Registry registry, Stream stream)
		{
			var entities = registry.Entities;

			entities.Count = SerializationHelpers.ReadInt(stream);
			entities.MaxId = SerializationHelpers.ReadInt(stream);

			entities.ResizePacked(entities.MaxId);
			entities.ResizeSparse(entities.MaxId);

			stream.Read(MemoryMarshal.Cast<int, byte>(entities.Ids.AsSpan(0, entities.MaxId)));
			stream.Read(MemoryMarshal.Cast<uint, byte>(entities.Reuses.AsSpan(0, entities.MaxId)));
			stream.Read(MemoryMarshal.Cast<int, byte>(entities.Sparse.AsSpan(0, entities.MaxId)));
		}
	}
}
