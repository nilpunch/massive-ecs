using System;

namespace Massive
{
	public interface IDataSet : IIdsSource
	{
		IPagedArray Data { get; }
		Type DataType { get; }

		void CopyData(int sourceId, int targetId);

		object GetRaw(int id);
		void SetRaw(int id, object value);
	}
}
