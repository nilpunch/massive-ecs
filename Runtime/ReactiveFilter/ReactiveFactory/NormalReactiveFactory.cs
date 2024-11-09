namespace Massive
{
	public class NormalReactiveFactory : IReactiveFactory
	{
		public ReactiveFilter CreateReactiveFilter(SparseSet[] include = null, SparseSet[] exclude = null, Entities entities = null)
		{
			return new ReactiveFilter(include, exclude, entities);
		}
	}
}
