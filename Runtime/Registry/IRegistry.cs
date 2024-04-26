namespace Massive
{
	public interface IRegistry
	{
		IGroupsController Groups { get; }

		Entities Entities { get; }

		IDataSet<T> Components<T>();

		ISet Any<T>();
	}
}
