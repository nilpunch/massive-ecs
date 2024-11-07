using System.Collections.Generic;

namespace Massive
{
	public interface IReactiveFactory
	{
		ReactiveFilter CreateReactiveFilter(IReadOnlyList<SparseSet> include, IReadOnlyList<SparseSet> exclude = null, Entities entities = null);
	}
}
