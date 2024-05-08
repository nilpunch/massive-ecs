using System.IO;

namespace Massive.Serialization
{
	public interface IRegistrySerializer
	{
		void Serialize(IRegistry registry, Stream stream);
		void Deserialize(IRegistry registry, Stream stream);
	}
}
