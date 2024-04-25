using System.Collections.Generic;
using System.IO;

namespace Massive.Serialization
{
	public class RegistryParser : IRegistryParser
	{
		private readonly List<IRegistryParser> _parsers = new List<IRegistryParser>();

		public RegistryParser()
		{
			_parsers.Add(new EntitiesParser());
		}

		public void AddComponent<T>() where T : unmanaged
		{
			_parsers.Add(new ComponentParser<T>());
		}

		public void AddCustomComponent<T>(IDataSetParser<T> dataParser = null) where T : struct
		{
			_parsers.Add(new CustomComponentParser<T>(dataParser ?? new DefaultDataSetParser<T>()));
		}

		public void AddNonOwningGroup(ComponentsGroup include = null, ComponentsGroup exclude = null)
		{
			_parsers.Add(new NonOwningGroupParser(include, exclude));
		}

		public void Write(IRegistry registry, Stream stream)
		{
			foreach (var parser in _parsers)
			{
				parser.Write(registry, stream);
			}
		}

		public void Read(IRegistry registry, Stream stream)
		{
			foreach (var parser in _parsers)
			{
				parser.Read(registry, stream);
			}
		}
	}
}