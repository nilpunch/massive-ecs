using System.Collections.Generic;

namespace Massive
{
	public interface IGroupFactory
	{
		IOwningGroup CreateOwningGroup(IReadOnlyList<ISet> owned, IReadOnlyList<IReadOnlySet> include = null, IReadOnlyList<IReadOnlySet> exclude = null);
		IGroup CreateNonOwningGroup(IReadOnlyList<IReadOnlySet> include, IReadOnlyList<IReadOnlySet> exclude = null);
	}
}
