using System.Collections.Generic;

namespace Massive
{
	public class NormalReactiveFactory : IReactiveFactory
	{
		public ReactiveFilter CreateReactiveFilter(IReadOnlyList<SparseSet> include, IReadOnlyList<SparseSet> exclude = null, Entities entities = null)
		{
			return new ReactiveFilter(include, exclude, entities);
		}
	}
}
