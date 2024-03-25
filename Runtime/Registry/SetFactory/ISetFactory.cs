namespace Massive
{
	public interface ISetFactory
	{
		Identifiers CreateIdentifiers();

		ISet CreateAppropriateSet<T>() where T : struct;

		ISet CreateSparseSet();

		ISet CreateDataSet<T>() where T : struct;
	}
}