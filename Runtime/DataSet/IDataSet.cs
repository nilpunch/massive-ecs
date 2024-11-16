namespace Massive
{
	public interface IDataSet
	{
		IPagedArray Data { get; }

		object GetRaw(int id);
		void SetRaw(int id, object value);
	}
}
