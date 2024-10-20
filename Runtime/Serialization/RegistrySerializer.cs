using System;
using System.Collections.Generic;
using System.IO;

namespace Massive.Serialization
{
	public class RegistrySerializer : IRegistrySerializer
	{
		private readonly List<IRegistrySerializer> _serializers = new List<IRegistrySerializer>();
		private readonly HashSet<Type> _serializedTypes = new HashSet<Type>();

		public RegistrySerializer()
		{
			_serializers.Add(new EntitiesSerializer());
		}

		public void AddComponent<T>() where T : unmanaged
		{
			if (_serializedTypes.Contains(typeof(T)))
			{
				throw new Exception($"Serializer for {typeof(T).Name} component has already been added!");
			}

			_serializedTypes.Add(typeof(T));
			_serializers.Add(new ComponentSerializer<T>());
		}

		public void AddCustomComponent<T>(IDataSetSerializer<T> dataSerializer = null)
		{
			if (_serializedTypes.Contains(typeof(T)))
			{
				throw new Exception($"Serializer for {typeof(T).Name} component has already been added!");
			}

			_serializedTypes.Add(typeof(T));
			_serializers.Add(new CustomComponentSerializer<T>(dataSerializer ?? new DefaultDataSetSerializer<T>()));
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
			foreach (var parser in _serializers)
			{
				parser.Serialize(registry, stream);
			}
		}

		public void Deserialize(Registry registry, Stream stream)
		{
			foreach (var set in registry.SetRegistry.All)
			{
				set.Clear();
			}

			foreach (var parser in _serializers)
			{
				parser.Deserialize(registry, stream);
			}
		}
	}
}
