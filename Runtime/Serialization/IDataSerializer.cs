using System.IO;

namespace Massive.Serialization
{
	public interface IDataSerializer
	{
		void Write(IPagedArray pagedArray, int count, Stream stream);
		void Read(IPagedArray pagedArray, int count, Stream stream);
	}
}
