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
			List<Group> syncedGroups = new List<Group>();
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
				SparseSet[] includeSets;
				SparseSet[] excludeSets;
				SparseSet[] ownSets;

				if (group is NonOwningGroup nonOwningGroup)
				{
					includeSets = nonOwningGroup.Include;
					excludeSets = nonOwningGroup.Exclude;
					ownSets = Array.Empty<SparseSet>();
				}
				else
				{
					var owningGroup = (OwningGroup)group;
					includeSets = owningGroup.Include;
					excludeSets = owningGroup.Exclude;
					ownSets = owningGroup.Owned;
				}

				// Include
				SerializationHelpers.WriteInt(includeSets.Length, stream);
				foreach (var set in includeSets)
				{
					var setKey = registry.SetRegistry.GetKey(set);
					SerializationHelpers.WriteType(setKey, stream);
				}

				// Exclude
				SerializationHelpers.WriteInt(excludeSets.Length, stream);
				foreach (var set in excludeSets)
				{
					var setKey = registry.SetRegistry.GetKey(set);
					SerializationHelpers.WriteType(setKey, stream);
				}

				// Own
				SerializationHelpers.WriteInt(ownSets.Length, stream);
				foreach (var set in ownSets)
				{
					var setKey = registry.SetRegistry.GetKey(set);
					SerializationHelpers.WriteType(setKey, stream);
				}

				if (group is NonOwningGroup)
				{
					SerializationHelpers.WriteSparseSet(group.MainSet, stream);
				}
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
			for (var i = 0; i < groupCount; i++)
			{
				// Include
				SparseSet[] includeSets = new SparseSet[SerializationHelpers.ReadInt(stream)];
				for (int setIndex = 0; setIndex < includeSets.Length; setIndex++)
				{
					includeSets[setIndex] = registry.SetRegistry.Get(SerializationHelpers.ReadType(stream));
				}

				// Exclude
				SparseSet[] excludeSets = new SparseSet[SerializationHelpers.ReadInt(stream)];
				for (int setIndex = 0; setIndex < excludeSets.Length; setIndex++)
				{
					excludeSets[setIndex] = registry.SetRegistry.Get(SerializationHelpers.ReadType(stream));
				}

				// Own
				SparseSet[] ownSets = new SparseSet[SerializationHelpers.ReadInt(stream)];
				for (int setIndex = 0; setIndex < ownSets.Length; setIndex++)
				{
					ownSets[setIndex] = registry.SetRegistry.Get(SerializationHelpers.ReadType(stream));
				}

				var group = registry.GroupRegistry.Get(includeSets, excludeSets, ownSets);
				deserializedGroups.Add(group);

				if (group is NonOwningGroup nonOwningGroup)
				{
					SerializationHelpers.ReadSparseSet(group.MainSet, stream);
					nonOwningGroup.SyncCount();
				}
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
