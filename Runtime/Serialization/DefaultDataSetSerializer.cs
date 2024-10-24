using System.IO;

namespace Massive.Serialization
{
	public class DefaultDataSetSerializer : IDataSetSerializer
	{
		public void Write(IDataSet set, Stream stream)
		{
			SerializationHelpers.WriteManagedPagedArray(set.Data, set.Count, stream);
		}

		public void Read(IDataSet set, Stream stream)
		{
			SerializationHelpers.ReadManagedPagedArray(set.Data, set.Count, stream);
		}
	}
}
