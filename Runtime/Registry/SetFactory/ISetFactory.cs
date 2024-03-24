namespace Massive
{
	public interface ISetFactory
	{
		ISet CreateSet();

		IDataSet<T> CreateDataSet<T>() where T : struct;

		Identifiers CreateIdentifiers();
	}
}