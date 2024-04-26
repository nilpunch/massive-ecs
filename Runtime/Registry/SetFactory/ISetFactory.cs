namespace Massive
{
	public interface ISetFactory
	{
		Entities CreateIdentifiers();

		ISet CreateAppropriateSet<T>();

		ISet CreateSparseSet();

		ISet CreateDataSet<T>();
	}
}
