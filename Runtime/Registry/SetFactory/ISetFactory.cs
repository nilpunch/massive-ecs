namespace Massive
{
	public interface ISetFactory
	{
		Entities CreateEntities();

		ISet CreateAppropriateSet<T>();

		ISet CreateSparseSet();

		ISet CreateDataSet<T>();
	}
}
