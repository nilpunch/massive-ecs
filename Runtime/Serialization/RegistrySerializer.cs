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

			// Reactive filters
			List<ReactiveFilter> syncedFilters = new List<ReactiveFilter>();
			foreach (var reactiveFilter in registry.ReactiveRegistry.All)
			{
				if (reactiveFilter.IsSynced)
				{
					syncedFilters.Add(reactiveFilter);
				}
			}
			SerializationHelpers.WriteInt(syncedFilters.Count, stream);
			foreach (ReactiveFilter reactiveFilter in syncedFilters)
			{
				SparseSet[] included = reactiveFilter.Included;
				SparseSet[] excluded = reactiveFilter.Excluded;

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

				SerializationHelpers.WriteSparseSet(reactiveFilter.Set, stream);
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

			// Reactive filters
			var deserializedFilters = new HashSet<ReactiveFilter>();
			var filterCount = SerializationHelpers.ReadInt(stream);
			for (var filterIndex = 0; filterIndex < filterCount; filterIndex++)
			{
				// Included
				SparseSet[] included = new SparseSet[SerializationHelpers.ReadInt(stream)];
				for (int i = 0; i < included.Length; i++)
				{
					included[i] = registry.SetRegistry.Get(SerializationHelpers.ReadType(stream));
				}

				// Excluded
				SparseSet[] excluded = new SparseSet[SerializationHelpers.ReadInt(stream)];
				for (int i = 0; i < excluded.Length; i++)
				{
					excluded[i] = registry.SetRegistry.Get(SerializationHelpers.ReadType(stream));
				}

				var reactiveFilter = registry.ReactiveFilter(included, excluded);
				deserializedFilters.Add(reactiveFilter);

				SerializationHelpers.ReadSparseSet(reactiveFilter.Set, stream);
			}
			// Desync all remaining filters
			foreach (var reactiveFilter in registry.ReactiveRegistry.All)
			{
				if (!deserializedFilters.Contains(reactiveFilter))
				{
					reactiveFilter.Desync();
				}
			}
		}
	}
}
