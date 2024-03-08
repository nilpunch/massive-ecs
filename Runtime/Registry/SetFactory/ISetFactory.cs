namespace Massive
{
	public interface ISetFactory
	{
		ISet CreateSet();
		ISet CreateDataSet<T>() where T : struct;
		Identifiers CreateIdentifiers();
	}
}