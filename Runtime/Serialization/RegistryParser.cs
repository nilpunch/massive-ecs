using System.Collections.Generic;
using System.IO;

namespace Massive.Serialization
{
	public class RegistryParser : IRegistryParser
	{
		private readonly List<IRegistryParser> _componentParsers = new List<IRegistryParser>();
		private readonly List<IRegistryParser> _nonOwningGroupParsers = new List<IRegistryParser>();

		public void AddComponent<T>() where T : unmanaged
		{
			_componentParsers.Add(new ComponentParser<T>());
		}

		public void AddCustomComponent<T>(IDataSetParser<T> dataParser) where T : struct
		{
			_componentParsers.Add(new CustomComponentParser<T>(dataParser));
		}

		public void AddNonOwningGroup(ComponentsGroup include = null, ComponentsGroup exclude = null)
		{
			_nonOwningGroupParsers.Add(new NonOwningGroupParser(include, exclude));
		}

		public void Write(IRegistry registry, Stream stream)
		{
			foreach (var parser in _componentParsers)
			{
				parser.Write(registry, stream);
			}

			foreach (var parser in _nonOwningGroupParsers)
			{
				parser.Write(registry, stream);
			}
		}

		public void Read(IRegistry registry, Stream stream)
		{
			foreach (var parser in _componentParsers)
			{
				parser.Read(registry, stream);
			}

			foreach (var parser in _nonOwningGroupParsers)
			{
				parser.Read(registry, stream);
			}
		}
	}
}