namespace Massive.ECS
{
	public interface ISetFactory<TSet> where TSet : ISet
	{
		TSet CreateSet();
		TSet CreateDataSet<T>() where T : unmanaged;
	}
}