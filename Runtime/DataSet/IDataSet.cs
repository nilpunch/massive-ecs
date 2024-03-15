namespace Massive
{
	public interface IDataSet<T> : IReadOnlyDataSet<T>, ISet where T : struct
	{
		EnsureInfo Ensure(int id, T data);
	}
}