namespace Massive
{
	public interface IGroupsController
	{
		IGroup EnsureOwningGroup(ISet[] owned, ISet[] other = null, IFilter filter = null);
		IGroup EnsureNonOwningGroup(ISet[] other, IFilter filter = null);
	}
}