using System.IO;

namespace Massive.Serialization
{
	public interface IRegistrySerializer
	{
		void Serialize(Registry registry, Stream stream);
		void Deserialize(Registry registry, Stream stream);
	}
}
