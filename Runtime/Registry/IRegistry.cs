namespace Massive
{
	public interface IRegistry
	{
		IGroupsController Groups { get; }

		IEntities Entities { get; }

		IDataSet<T> Components<T>();

		ISet Any<T>();
	}
}
