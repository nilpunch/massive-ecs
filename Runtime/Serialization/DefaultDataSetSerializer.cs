using System;
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

			var pagedData = set.Data;
			foreach (var (pageIndex, pageLength, indexOffset) in new PageSequence(pagedData.PageSize, set.Count))
			{
				Array.Copy(pagedData.Pages[pageIndex], 0, data, indexOffset, pageLength);
			}

			_binaryFormatter.Serialize(stream, data);
		}

		public void Read(DataSet<T> set, Stream stream)
		{
			var data = (T[])_binaryFormatter.Deserialize(stream);

			var pagedData = set.Data;
			foreach (var (pageIndex, pageLength, indexOffset) in new PageSequence(pagedData.PageSize, data.Length))
			{
				pagedData.EnsurePage(pageIndex);
				Array.Copy(data, indexOffset, pagedData.Pages[pageIndex], 0, pageLength);
			}
		}
	}
}
