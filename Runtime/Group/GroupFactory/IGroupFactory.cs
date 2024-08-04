using System.Collections.Generic;

namespace Massive
{
	public interface IGroupFactory
	{
		IOwningGroup CreateOwningGroup(IReadOnlyList<SparseSet> owned, IReadOnlyList<SparseSet> include = null, IReadOnlyList<SparseSet> exclude = null);
		IGroup CreateNonOwningGroup(IReadOnlyList<SparseSet> include, IReadOnlyList<SparseSet> exclude = null);
	}
}
