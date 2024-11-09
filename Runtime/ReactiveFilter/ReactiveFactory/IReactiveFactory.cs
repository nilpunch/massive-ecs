namespace Massive
{
	public interface IReactiveFactory
	{
		ReactiveFilter CreateReactiveFilter(SparseSet[] include = null, SparseSet[] exclude = null, Entities entities = null);
	}
}
