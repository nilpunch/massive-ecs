namespace Massive
{
	public interface IDataSet<T> : IReadOnlyDataSet<T>, ISet where T : struct
	{
		T[] RawData { get; }

		void Assign(int id, T data);
	}
}
