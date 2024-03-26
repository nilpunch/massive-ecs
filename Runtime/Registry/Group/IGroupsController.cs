namespace Massive
{
	public interface IGroupsController
	{
		IGroup EnsureGroup(ISet[] owned, ISet[] other = null, IFilter filter = null);
	}
}