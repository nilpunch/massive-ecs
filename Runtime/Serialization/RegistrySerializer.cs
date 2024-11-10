using System;
using System.Collections.Generic;
using System.IO;

namespace Massive.Serialization
{
	public class RegistrySerializer : IRegistrySerializer
	{
		private readonly Dictionary<Type, IDataSerializer> _customSerializers = new Dictionary<Type, IDataSerializer>();

		public void AddCustomSerializer(Type type, IDataSerializer dataSerializer)
		{
			_customSerializers[type] = dataSerializer;
		}

		public void Serialize(Registry registry, Stream stream)
		{
			// Entities
			SerializationHelpers.WriteEntities(registry.Entities, stream);

			// Sets
			SerializationHelpers.WriteInt(registry.SetRegistry.All.Length, stream);
			foreach (var sparseSet in registry.SetRegistry.All)
			{
				var setKey = registry.SetRegistry.GetKey(sparseSet);
				SerializationHelpers.WriteType(setKey, stream);
				SerializationHelpers.WriteSparseSet(sparseSet, stream);

				if (sparseSet is not IDataSet dataSet)
				{
					continue;
				}

				if (_customSerializers.TryGetValue(setKey, out var customSerializer))
				{
					customSerializer.Write(dataSet.Data, sparseSet.Count, stream);
					continue;
				}

				if (dataSet.Data.ElementType.IsUnmanaged())
				{
					SerializationHelpers.WriteUnmanagedPagedArray(dataSet.Data, sparseSet.Count, stream);
				}
				else
				{
					SerializationHelpers.WriteManagedPagedArray(dataSet.Data, sparseSet.Count, stream);
				}
			}

			// Groups
			var syncedGroups = new List<Group>();
			foreach (var group in registry.GroupRegistry.All)
			{
				if (group.IsSynced)
				{
					syncedGroups.Add(group);
				}
			}
			SerializationHelpers.WriteInt(syncedGroups.Count, stream);
			foreach (var group in syncedGroups)
			{
				var included = group.Included;
				var excluded = group.Excluded;

				// Included
				SerializationHelpers.WriteInt(included.Length, stream);
				foreach (var set in included)
				{
					var setKey = registry.SetRegistry.GetKey(set);
					SerializationHelpers.WriteType(setKey, stream);
				}

				// Excluded
				SerializationHelpers.WriteInt(excluded.Length, stream);
				foreach (var set in excluded)
				{
					var setKey = registry.SetRegistry.GetKey(set);
					SerializationHelpers.WriteType(setKey, stream);
				}

				SerializationHelpers.WriteSparseSet(group.Set, stream);
			}
		}

		public void Deserialize(Registry registry, Stream stream)
		{
			// Entities
			SerializationHelpers.ReadEntities(registry.Entities, stream);

			// Sets
			var deserializedSets = new HashSet<SparseSet>();
			var setCount = SerializationHelpers.ReadInt(stream);
			for (var i = 0; i < setCount; i++)
			{
				var setKey = SerializationHelpers.ReadType(stream);

				var sparseSet = registry.SetRegistry.Get(setKey);
				deserializedSets.Add(sparseSet);

				SerializationHelpers.ReadSparseSet(sparseSet, stream);

				if (sparseSet is not IDataSet dataSet)
				{
					continue;
				}

				if (_customSerializers.TryGetValue(setKey, out var customSerializer))
				{
					customSerializer.Read(dataSet.Data, sparseSet.Count, stream);
					continue;
				}

				if (dataSet.Data.ElementType.IsUnmanaged())
				{
					SerializationHelpers.ReadUnmanagedPagedArray(dataSet.Data, sparseSet.Count, stream);
				}
				else
				{
					SerializationHelpers.ReadManagedPagedArray(dataSet.Data, sparseSet.Count, stream);
				}
			}
			// Clear all remaining sets
			foreach (var sparseSet in registry.SetRegistry.All)
			{
				if (!deserializedSets.Contains(sparseSet))
				{
					sparseSet.ClearWithoutNotify();
				}
			}

			// Groups
			var deserializedGroups = new HashSet<Group>();
			var groupCount = SerializationHelpers.ReadInt(stream);
			for (var groupIndex = 0; groupIndex < groupCount; groupIndex++)
			{
				// Included
				var included = new SparseSet[SerializationHelpers.ReadInt(stream)];
				for (var i = 0; i < included.Length; i++)
				{
					included[i] = registry.SetRegistry.Get(SerializationHelpers.ReadType(stream));
				}

				// Excluded
				var excluded = new SparseSet[SerializationHelpers.ReadInt(stream)];
				for (var i = 0; i < excluded.Length; i++)
				{
					excluded[i] = registry.SetRegistry.Get(SerializationHelpers.ReadType(stream));
				}

				var group = registry.Group(included, excluded);
				deserializedGroups.Add(group);

				SerializationHelpers.ReadSparseSet(group.Set, stream);
			}
			// Desync all remaining groups
			foreach (var group in registry.GroupRegistry.All)
			{
				if (!deserializedGroups.Contains(group))
				{
					group.Desync();
				}
			}
		}
	}
}
