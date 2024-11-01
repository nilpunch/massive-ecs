using System.Collections.Generic;

namespace Massive
{
	public class NormalGroupFactory : IGroupFactory
	{
		public OwningGroup CreateOwningGroup(IReadOnlyList<SparseSet> owned, IReadOnlyList<SparseSet> include = null, IReadOnlyList<SparseSet> exclude = null)
		{
			return new OwningGroup(owned, include, exclude);
		}

		public NonOwningGroup CreateNonOwningGroup(IReadOnlyList<SparseSet> include, IReadOnlyList<SparseSet> exclude = null, Entities entities = null)
		{
			return new NonOwningGroup(include, exclude, entities);
		}
	}
}
