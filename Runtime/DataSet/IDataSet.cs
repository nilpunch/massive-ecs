namespace Massive
{
	public interface IDataSet<T> : IReadOnlyDataSet<T>, ISet where T : struct
	{
		int Ensure(int id, T data);
	}
}