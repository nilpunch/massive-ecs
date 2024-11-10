namespace Massive
{
	public interface IGroupFactory
	{
		Group CreateGroup(SparseSet[] include = null, SparseSet[] exclude = null, Entities entities = null);
	}
}
