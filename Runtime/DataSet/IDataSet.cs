namespace Massive
{
	public interface IDataSet<T> : IReadOnlyDataSet<T>, ISet where T : struct
	{
		void Ensure(int id, T data);
	}
}