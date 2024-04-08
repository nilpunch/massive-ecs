namespace Massive
{
	public interface ISetFactory
	{
		Entities CreateIdentifiers();

		ISet CreateAppropriateSet<T>() where T : struct;

		ISet CreateSparseSet();

		ISet CreateDataSet<T>() where T : struct;
	}
}