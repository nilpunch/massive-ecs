using System.IO;

namespace Massive.Serialization
{
	public class DefaultDataSerializer : IDataSerializer
	{
		public void Write(IPagedArray pagedArray, int count, Stream stream)
		{
			SerializationHelpers.WriteManagedPagedArray(pagedArray, count, stream);
		}

		public void Read(IPagedArray pagedArray, int count, Stream stream)
		{
			SerializationHelpers.ReadManagedPagedArray(pagedArray, count, stream);
		}
	}
}
