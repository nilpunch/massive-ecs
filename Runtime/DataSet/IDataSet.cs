using System;

namespace Massive
{
	public interface IDataSet : IIdsSource
	{
		void CopyData(int sourceId, int targetId);

		Type GetDataType();
		object GetRaw(int id);
		void SetRaw(int id, object value);
	}
}
