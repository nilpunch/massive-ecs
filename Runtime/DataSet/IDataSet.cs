namespace Massive
{
	public interface IDataSet<T> : IReadOnlyDataSet<T>, ISet
	{
		void Assign(int id, T data);
	}
}
