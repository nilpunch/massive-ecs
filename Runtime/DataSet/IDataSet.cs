namespace Massive
{
	public interface IDataSet
	{
		IPagedArray Data { get; }

		void CopyData(int sourceId, int targetId);

		object GetRaw(int id);
		void SetRaw(int id, object value);
	}
}
