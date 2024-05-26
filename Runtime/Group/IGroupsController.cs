namespace Massive
{
	public interface IGroupsController
	{
		IGroup EnsureGroup<TGroupSelector>(TGroupSelector groupSelector) where TGroupSelector : IGroupSelector;
	}
}
