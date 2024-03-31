using System;

namespace Massive
{
	public interface IGroup
	{
		ISet[] Owned { get; }

		IReadOnlySet[] Include { get; }

		IReadOnlySet[] Exclude { get; }

		ReadOnlySpan<int> Ids { get; }

		int Length => Ids.Length;

		void EnsureSynced();

		bool IsOwning(IReadOnlySet set);

		bool ExtendsGroup(ISet[] owned, IReadOnlySet[] include, IReadOnlySet[] exclude);

		bool BaseForGroup(ISet[] owned, IReadOnlySet[] include, IReadOnlySet[] exclude);
	}
}