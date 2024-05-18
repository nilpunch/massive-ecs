using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Massive.Serialization
{
	public class DefaultDataSetSerializer<T> : IDataSetSerializer<T>
	{
		private readonly BinaryFormatter _binaryFormatter = new BinaryFormatter();

		public void Write(DataSet<T> set, Stream stream)
		{
			var data = new T[set.Count];

			// Array.Copy(set.RawData, data, set.Count);

			_binaryFormatter.Serialize(stream, data);
		}

		public void Read(DataSet<T> set, Stream stream)
		{
			var data = (T[])_binaryFormatter.Deserialize(stream);

			// Array.Copy(data, set.RawData, data.Length);
		}
	}
}
