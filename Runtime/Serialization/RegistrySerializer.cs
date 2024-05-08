using System.Collections.Generic;
using System.IO;

namespace Massive.Serialization
{
	public class RegistrySerializer : IRegistrySerializer
	{
		private readonly List<IRegistrySerializer> _parsers = new List<IRegistrySerializer>();

		public RegistrySerializer()
		{
			_parsers.Add(new EntitiesSerializer());
		}

		public void AddComponent<T>() where T : unmanaged
		{
			_parsers.Add(new ComponentSerializer<T>());
		}

		public void AddCustomComponent<T>(IDataSetSerializer<T> dataSerializer = null)
		{
			_parsers.Add(new CustomComponentSerializer<T>(dataSerializer ?? new DefaultDataSetSerializer<T>()));
		}

		public void AddNonOwningGroup(SetSelector includeSets = null, SetSelector excludeSets = null)
		{
			_parsers.Add(new NonOwningGroupSerializer(includeSets, excludeSets));
		}

		public void Serialize(IRegistry registry, Stream stream)
		{
			foreach (var parser in _parsers)
			{
				parser.Serialize(registry, stream);
			}
		}

		public void Deserialize(IRegistry registry, Stream stream)
		{
			foreach (var parser in _parsers)
			{
				parser.Deserialize(registry, stream);
			}
		}
	}
}
