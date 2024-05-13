using System;

namespace Massive
{
	public interface IGroup
	{
		ReadOnlyPackedSpan<int> Ids { get; }

		int Length => Ids.Length;

		bool IsSynced { get; }

		void EnsureSynced();

		bool IsOwning(IReadOnlySet set);
	}
}
