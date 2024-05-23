namespace Massive
{
	public interface IRegistry
	{
		IndexedSetCollection SetCollection { get; }
		IGroupsController Groups { get; }
		IEntities Entities { get; }
	}
}
