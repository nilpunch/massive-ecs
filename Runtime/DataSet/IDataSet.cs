namespace Massive
{
	public interface IDataSet<T> : IReadOnlyDataSet<T>, ISet where T : struct
	{
		CreateInfo Ensure(int id, T data);
	}
}