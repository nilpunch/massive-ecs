using System;

namespace Massive
{
	public interface IGroup
	{
		ISet[] Owned { get; }

		IReadOnlySet[] Include { get; }

		IReadOnlySet[] Exclude { get; }

		ReadOnlySpan<int> GroupIds { get; }

		void EnsureSynced();

		bool IsOwning(IReadOnlySet set);

		bool ExtendsGroup(ISet[] owned, IReadOnlySet[] include, IReadOnlySet[] exclude);

		bool BaseForGroup(ISet[] owned, IReadOnlySet[] include, IReadOnlySet[] exclude);
	}
}