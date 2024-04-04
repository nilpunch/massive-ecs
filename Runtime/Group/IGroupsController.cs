namespace Massive
{
	public interface IGroupsController
	{
		IGroup EnsureGroup(ISet[] owned = null, IReadOnlySet[] include = null, IReadOnlySet[] exclude = null);
	}
}