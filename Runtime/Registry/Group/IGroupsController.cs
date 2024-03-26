namespace Massive
{
	public interface IGroupsController
	{
		IGroup EnsureOwningGroup(ISet[] owned, ISet[] other = null);
		IGroup EnsureNonOwningGroup(ISet[] other);
	}
}