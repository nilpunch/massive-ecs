namespace Massive
{
	public interface IRegistry
	{
		IGroupsController Groups { get; }

		Entities Entities { get; }

		IDataSet<T> Components<T>() where T : struct;

		ISet Any<T>() where T : struct;
	}
}
