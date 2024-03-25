namespace Massive
{
	public interface ISetFactory
	{
		ISet CreateSet<T>() where T : struct;

		Identifiers CreateIdentifiers();
	}
}