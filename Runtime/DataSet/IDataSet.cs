namespace Massive
{
	public interface IDataSet<T> : IReadOnlyDataSet<T>, ISet where T : struct
	{
		T[] Data { get; }

		void Ensure(int id, T data);
	}
}