namespace Massive
{
	public interface IDataSet<T> : IReadOnlyDataSet<T>, ISet
	{
		T[] RawData { get; }

		void Assign(int id, T data);
	}
}
