namespace Massive
{
	public interface IDataSet : ISet
	{
		void CopyData(int sourceId, int targetId);
	}

	public interface IDataSet<T> : IDataSet, IReadOnlyDataSet<T>
	{
		void Assign(int id, T data);
	}
}
