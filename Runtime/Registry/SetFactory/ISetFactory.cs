namespace Massive.ECS
{
	public interface ISetFactory
	{
		ISet CreateSet();
		ISet CreateDataSet<T>() where T : unmanaged;
	}
}