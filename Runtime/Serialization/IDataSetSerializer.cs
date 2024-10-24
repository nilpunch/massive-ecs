using System.IO;

namespace Massive.Serialization
{
	public interface IDataSetSerializer
	{
		void Write(IDataSet set, Stream stream);
		void Read(IDataSet set, Stream stream);
	}
}
