namespace Massive
{
	public interface ISetFactory
	{
		ISet CreateSet();
		ISet CreateDataSet<T>() where T : struct;
		ISet CreateManagedDataSet<T>() where T : struct, IManaged<T>;
		Identifiers CreateIdentifiers();
	}
}