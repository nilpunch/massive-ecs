using System.IO;

namespace Massive.Serialization
{
	public interface IDataSetSerializer<T>
	{
		void Write(DataSet<T> set, Stream stream);
		void Read(DataSet<T> set, Stream stream);
	}
}
