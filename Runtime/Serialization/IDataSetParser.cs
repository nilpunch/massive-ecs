using System.IO;

namespace Massive.Serialization
{
	public interface IDataSetParser<T> where T : struct
	{
		void Write(IDataSet<T> set, Stream stream);
		void Read(IDataSet<T> set, Stream stream);
	}
}
