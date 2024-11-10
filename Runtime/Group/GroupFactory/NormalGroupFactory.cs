namespace Massive
{
	public class NormalGroupFactory : IGroupFactory
	{
		public Group CreateGroup(SparseSet[] include = null, SparseSet[] exclude = null, Entities entities = null)
		{
			return new Group(include, exclude, entities);
		}
	}
}
