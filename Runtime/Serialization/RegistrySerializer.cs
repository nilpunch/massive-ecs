using System;
using System.Collections.Generic;
using System.IO;

namespace Massive.Serialization
{
	public class RegistrySerializer : IRegistrySerializer
	{
		private readonly Dictionary<Type, IDataSetSerializer> _customSerializers = new Dictionary<Type, IDataSetSerializer>();
		private readonly List<IRegistrySerializer> _serializers = new List<IRegistrySerializer>();
		private readonly HashSet<Type> _serializedTypes = new HashSet<Type>();

		public RegistrySerializer()
		{
			_serializers.Add(new EntitiesSerializer());
		}

		public void AddCustomSerializer(Type type, IDataSetSerializer dataSetSerializer)
		{
			_customSerializers[type] = dataSetSerializer;
		}

		public void AddNonOwningGroup<TInclude>()
			where TInclude : IIncludeSelector, new()
		{
			AddNonOwningGroup<TInclude, None>();
		}

		public void AddNonOwningGroup<TInclude, TExclude>()
			where TInclude : IIncludeSelector, new()
			where TExclude : IExcludeSelector, new()
		{
			_serializers.Add(new NonOwningGroupSerializer<TInclude, TExclude>());
		}

		public void Serialize(Registry registry, Stream stream)
		{
			SerializationHelpers.WriteInt(registry.SetRegistry.All.Length, stream);
			foreach (var sparseSet in registry.SetRegistry.All)
			{
				var setType = registry.SetRegistry.TypeOf(sparseSet);
			
				SerializationHelpers.WriteType(setType, stream);
				SerializationHelpers.WriteSparseSet(sparseSet, stream);

				if (sparseSet is not IDataSet dataSet)
				{
					continue;
				}

				if (_customSerializers.TryGetValue(setType, out var customSerializer))
				{
					customSerializer.Write(dataSet, stream);
					continue;
				}

				if (dataSet.DataType.IsUnmanaged())
				{
					SerializationHelpers.WriteUnmanagedPagedArray(dataSet.Data, dataSet.Count, stream);
				}
				else
				{
					SerializationHelpers.WriteManagedPagedArray(dataSet.Data, dataSet.Count, stream);
				}
			}

			foreach (var registrySerializer in _serializers)
			{
				registrySerializer.Serialize(registry, stream);
			}
		}

		public void Deserialize(Registry registry, Stream stream)
		{
			foreach (var set in registry.SetRegistry.All)
			{
				set.Clear();
			}

			var setCount = SerializationHelpers.ReadInt(stream);
			for (var i = 0; i < setCount; i++)
			{
				var setType = SerializationHelpers.ReadType(stream);
				
				var sparseSet = registry.SetRegistry.Get(setType);

				SerializationHelpers.ReadSparseSet(sparseSet, stream);

				if (sparseSet is not IDataSet dataSet)
				{
					continue;
				}

				if (_customSerializers.TryGetValue(setType, out var customSerializer))
				{
					customSerializer.Read(dataSet, stream);
					continue;
				}

				if (dataSet.DataType.IsUnmanaged())
				{
					SerializationHelpers.ReadUnmanagedPagedArray(dataSet.Data, dataSet.Count, stream);
				}
				else
				{
					SerializationHelpers.ReadManagedPagedArray(dataSet.Data, dataSet.Count, stream);
				}
			}

			foreach (var registrySerializer in _serializers)
			{
				registrySerializer.Deserialize(registry, stream);
			}
		}
	}
}
