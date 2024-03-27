namespace Massive
{
	public interface IGroupsController
	{
		IGroup EnsureGroup(ISet[] owned = null, ISet[] other = null, IFilter filter = null);
	}
}