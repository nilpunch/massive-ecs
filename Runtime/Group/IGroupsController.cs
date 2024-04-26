using System.Collections.Generic;

namespace Massive
{
	public interface IGroupsController
	{
		IGroup EnsureGroup(IReadOnlyList<ISet> owned = null, IReadOnlyList<IReadOnlySet> include = null, IReadOnlyList<IReadOnlySet> exclude = null);
	}
}
