namespace Massive
{
	public interface IDataSet<T> : IReadOnlyDataSet<T>, ISet where T : unmanaged
	{
		CreateInfo Ensure(int id, T data);
	}
}