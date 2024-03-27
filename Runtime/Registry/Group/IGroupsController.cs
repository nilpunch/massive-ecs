namespace Massive
{
	public interface IGroupsController
	{
		IGroup EnsureGroup(ISet[] owned = null, IReadOnlySet[] other = null, IFilter filter = null);
	}
}