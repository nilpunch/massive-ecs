using System.IO;

namespace Massive.Serialization
{
	public interface IRegistryParser
	{
		void Write(IRegistry registry, Stream stream);
		void Read(IRegistry registry, Stream stream);
	}
}